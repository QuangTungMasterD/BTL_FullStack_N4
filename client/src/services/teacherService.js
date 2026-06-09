import api from "./api";

const teacherService = {
  // Lấy danh sách có phân trang
  getTeachersPaged(params = {}) {
    return api.get("/Teachers/paged", { params });
  },
  // Lấy tất cả
  getAllTeachers() {
    return api.get("/Teachers");
  },
  // Lấy chi tiết
  getTeacherById(id) {
    return api.get(`/Teachers/${id}`);
  },
  // Tạo mới
  createTeacher(data) {
    return api.post("/Teachers", data);
  },
  // Cập nhật
  updateTeacher(id, data) {
    return api.put(`/Teachers/${id}`, data);
  },
  // Xóa
  deleteTeacher(id) {
    return api.delete(`/Teachers/${id}`);
  },
  deleteTeacherPermanent(id) {
    return api.delete(`/Teachers/${id}/permanent`);
  },

  // Khôi phục
  restoreTeacher(id) {
    return api.patch(`/Teachers/${id}/restore`);
  },

  async exportToExcel(params = {}) {
    const response = await this.getTeachersPaged({
      ...params,
      page: 1,
      pageSize: 10000,
    });

    const teachers = response.data || [];

    if (teachers.length === 0) {
      throw new Error("Không có dữ liệu để xuất");
    }

    // Dùng các hàm format từ useFormat nếu cần
    const excelData = teachers.map((teacher) => ({
      ID: teacher.id,
      "Họ tên": teacher.fullName,
      Email: teacher.email || "",
      "Số điện thoại": teacher.phone || "",
      "Năm sinh": teacher.yoB ? new Date(teacher.yoB).getFullYear() : "",
      "Giới tính":
        teacher.gender === true ? "Nam" : teacher.gender === false ? "Nữ" : "",
      "Trạng thái": teacher.isActive ? "Đang hoạt động" : "Ngừng hoạt động",
      "Chuyên ngành IDs": Array.isArray(teacher.specialization)
        ? teacher.specialization.join(", ")
        : "",
    }));

    const worksheet = XLSX.utils.json_to_sheet(excelData);
    worksheet["!cols"] = [
      { wch: 10 }, // ID
      { wch: 30 }, // Họ tên
      { wch: 30 }, // Email
      { wch: 15 }, // SĐT
      { wch: 12 }, // Năm sinh
      { wch: 10 }, // Giới tính
      { wch: 15 }, // Trạng thái
      { wch: 20 }, // Chuyên ngành IDs
    ];

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Teachers");
    const excelBuffer = XLSX.write(workbook, {
      bookType: "xlsx",
      type: "array",
    });
    const blob = new Blob([excelBuffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });
    saveAs(blob, `teachers_${new Date().toISOString().split("T")[0]}.xlsx`);

    return { success: true, count: teachers.length };
  },

  // Import Excel
  async importFromExcel(file) {
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
        const fullName = row["Họ tên"] || row["fullName"];
        const email = row["Email"] || row["email"] || "";
        const phone = row["Số điện thoại"] || row["phone"] || "";
        let yoB = row["Năm sinh"] || row["yoB"];
        // Xử lý năm sinh (có thể là số hoặc chuỗi)
        if (yoB) {
          if (typeof yoB === "number") {
            yoB = new Date(yoB, 0, 1).toISOString().split("T")[0];
          } else if (typeof yoB === "string" && yoB.match(/^\d{4}$/)) {
            yoB = `${yoB}-01-01`;
          }
        }
        const gender =
          row["Giới tính"] === "Nam"
            ? true
            : row["Giới tính"] === "Nữ"
              ? false
              : null;
        const isActive =
          row["Trạng thái"] === "Đang hoạt động"
            ? true
            : row["Trạng thái"] === "Ngừng hoạt động"
              ? false
              : true;

        // Validate
        if (!fullName) {
          throw new Error("Họ tên không được để trống");
        }
        if (email && !/^\S+@\S+\.\S+$/.test(email)) {
          throw new Error("Email không hợp lệ");
        }

        const teacherData = {
          fullName: fullName.trim(),
          email: email.trim(),
          phone: phone.trim(),
          yoB: yoB || null,
          gender: gender,
          isActive: isActive,
        };

        const result = await this.createTeacher(teacherData);
        results.success.push({ row: rowNumber, data: result });
      } catch (err) {
        results.errors.push({
          row: rowNumber,
          data: row,
          error: err.message,
        });
      }
    }

    return results;
  },
};

export default teacherService;
