import api from "./api";
import { classStatusText } from '@/composables/useFormat.js';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

const classService = {
  // Lấy danh sách có phân trang
  getClassesPaged(params = {}) {
    return api.get("/Classes/paged", { params });
  },
  // Lấy tất cả
  getAllClasses() {
    return api.get("/Classes");
  },
  // Lấy chi tiết
  getClassById(id) {
    return api.get(`/Classes/${id}`);
  },
  // Lấy lớp theo khóa học
  getClassesByCourseId(courseId) {
    return api.get(`/Classes/course/${courseId}`);
  },
  // Tạo mới
  createClass(data) {
    return api.post("/Classes", data);
  },
  // Cập nhật
  updateClass(id, data) {
    return api.put(`/Classes/${id}`, data);
  },
  // Xóa mềm
  deleteClass(id) {
    return api.delete(`/Classes/${id}`);
  },
  // Xóa vĩnh viễn
  deleteClassPermanent(id) {
    return api.delete(`/Classes/${id}/permanent`);
  },
  // Khôi phục
  restoreClass(id) {
    return api.patch(`/Classes/${id}/restore`);
  },
  // ---------- Export Excel (client-side) ----------
  async exportToExcel(params = {}) {
    const response = await this.getClassesPaged({
      ...params,
      page: 1,
      pageSize: 10000,
    });

    const classes = response.data || [];

    if (classes.length === 0) {
      throw new Error("Không có dữ liệu để xuất");
    }

    const excelData = classes.map((cls) => ({
      ID: cls.id,
      "Tên lớp": cls.className,
      "Sĩ số tối đa": cls.maxStudent,
      "Ngày bắt đầu": cls.startDate
        ? new Date(cls.startDate).toLocaleDateString("vi-VN")
        : "",
      "Ngày kết thúc": cls.endDate
        ? new Date(cls.endDate).toLocaleDateString("vi-VN")
        : "",
      "Trạng thái": classStatusText(cls.status),
      "Số buổi": cls.lesson,
      "ID khóa học": cls.courseId,
    }));

    const worksheet = XLSX.utils.json_to_sheet(excelData);
    worksheet["!cols"] = [
      { wch: 10 }, // ID
      { wch: 30 }, // Tên lớp
      { wch: 15 }, // Sĩ số
      { wch: 15 }, // Ngày bắt đầu
      { wch: 15 }, // Ngày kết thúc
      { wch: 18 }, // Trạng thái
      { wch: 10 }, // Số buổi
      { wch: 15 }, // ID khóa học
    ];

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Classes");
    const excelBuffer = XLSX.write(workbook, {
      bookType: "xlsx",
      type: "array",
    });
    const blob = new Blob([excelBuffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });
    saveAs(blob, `classes_${new Date().toISOString().split("T")[0]}.xlsx`);

    return { success: true, count: classes.length };
  },

  // ---------- Import Excel (client-side) ----------
  async importFromExcel(file, courseMap = {}) {
    const data = await file.arrayBuffer();
    const workbook = XLSX.read(data);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const rows = XLSX.utils.sheet_to_json(worksheet);

    if (rows.length === 0) {
      throw new Error("File Excel không có dữ liệu");
    }

    const results = {
      success: [],
      errors: [],
      total: rows.length,
    };

    for (let i = 0; i < rows.length; i++) {
      const row = rows[i];
      const rowNumber = i + 2;

      try {
        const className = row["Tên lớp"] || row["className"];
        const maxStudent = this._parseNumber(
          row["Sĩ số tối đa"] || row["maxStudent"],
        );
        const startDate = this._parseDate(
          row["Ngày bắt đầu"] || row["startDate"],
        );
        const endDate = this._parseDate(row["Ngày kết thúc"] || row["endDate"]);
        const statusText = row["Trạng thái"] || row["status"];
        const lesson = this._parseNumber(row["Số buổi"] || row["lesson"]);
        let courseId = this._parseNumber(row["ID khóa học"] || row["courseId"]);

        // Nếu không có courseId nhưng có tên khóa học trong courseMap
        if (
          !courseId &&
          row["Tên khóa học"] &&
          courseMap[row["Tên khóa học"]]
        ) {
          courseId = courseMap[row["Tên khóa học"]];
        }

        // Validate
        if (!className) throw new Error("Tên lớp không được để trống");
        if (isNaN(maxStudent) || maxStudent < 1)
          throw new Error("Sĩ số tối đa phải lớn hơn 0");
        if (!startDate) throw new Error("Ngày bắt đầu không hợp lệ");
        if (!endDate) throw new Error("Ngày kết thúc không hợp lệ");
        if (new Date(startDate) > new Date(endDate))
          throw new Error("Ngày bắt đầu phải trước ngày kết thúc");
        if (isNaN(lesson) || lesson < 1)
          throw new Error("Số buổi phải lớn hơn 0");
        if (!courseId) throw new Error("Không xác định được ID khóa học");

        const status = this._parseClassStatus(statusText);
        if (!status)
          throw new Error(
            "Trạng thái không hợp lệ (Chờ khai giảng, Đang học, Đã kết thúc, Đã hủy, Tạm dừng)",
          );

        const classData = {
          className: className.trim(),
          maxStudent: Number(maxStudent),
          startDate,
          endDate,
          status,
          lesson: Number(lesson),
          courseId: Number(courseId),
        };

        const result = await this.createClass(classData);
        results.success.push({ row: rowNumber, data: result });
      } catch (err) {
        results.errors.push({ row: rowNumber, data: row, error: err.message });
      }
    }

    return results;
  },

  // Helper methods
  _parseNumber(value) {
    if (value === undefined || value === null) return NaN;
    if (typeof value === "number") return value;
    const cleaned = String(value).replace(/[^\d.-]/g, "");
    const num = parseFloat(cleaned);
    return isNaN(num) ? NaN : num;
  },

  _parseDate(value) {
    if (!value) return null;
    let dateStr = String(value).trim();
    if (dateStr.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
      const parts = dateStr.split("/");
      dateStr = `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    const date = new Date(dateStr);
    return isNaN(date.getTime()) ? null : date.toISOString().split("T")[0];
  },

  _parseClassStatus(text) {
    if (!text) return null;
    const str = String(text).toLowerCase();
    const map = {
      "chờ khai giảng": 1,
      "đang học": 2,
      "đã kết thúc": 3,
      "đã hủy": 4,
      "tạm dừng": 5,
    };
    return map[str] || null;
  },
};

export default classService;
