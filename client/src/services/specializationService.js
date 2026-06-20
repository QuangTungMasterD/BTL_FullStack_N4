import api from './api';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

const specializationService = {
  // Lấy danh sách có phân trang
  getSpecializationsPaged(params = {}) {
    return api.get('v1/Specializations/paged', { params });
  },
  // Lấy tất cả
  getAllSpecializations() {
    return api.get('v1/Specializations');
  },
  // Lấy chi tiết
  getSpecializationById(id) {
    return api.get(`v1/Specializations/${id}`);
  },
  // Tạo mới
  createSpecialization(data) {
    return api.post('v1/Specializations', data);
  },
  // Cập nhật
  updateSpecialization(id, data) {
    return api.put(`v1/Specializations/${id}`, data);
  },
  // Xóa vĩnh viễn (không có xóa mềm theo Swagger)
  deleteSpecializationPermanent(id) {
    return api.delete(`v1/Specializations/${id}/permanent`);
  },
  // Export Excel (client-side)
  async exportToExcel(params = {}) {
    const response = await this.getSpecializationsPaged({
      ...params,
      page: 1,
      pageSize: 10000,
    });

    const specializations = response.data || [];

    if (specializations.length === 0) {
      throw new Error('Không có dữ liệu để xuất');
    }

    const excelData = specializations.map(spec => ({
      // 'ID': spec.id,
      'Tên chuyên ngành': spec.specializationName,
      'Mô tả': spec.descrt || '',
      'Trạng thái': spec.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động',
    }));

    const worksheet = XLSX.utils.json_to_sheet(excelData);
    worksheet['!cols'] = [
      // { wch: 10 }, // ID
      { wch: 35 }, // Tên chuyên ngành
      { wch: 50 }, // Mô tả
      { wch: 15 }, // Trạng thái
    ];

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Specializations');
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    saveAs(blob, `specializations_${new Date().toISOString().split('T')[0]}.xlsx`);

    return { success: true, count: specializations.length };
  },

  // Import Excel (client-side)
  async importFromExcel(file) {
    const data = await file.arrayBuffer();
    const workbook = XLSX.read(data);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const rows = XLSX.utils.sheet_to_json(worksheet);

    if (rows.length === 0) {
      throw new Error('File Excel không có dữ liệu');
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
        const specializationName = String(row['Tên chuyên ngành'] || row['specializationName'] || '').trim();
        if (!specializationName) throw new Error('Tên chuyên ngành không được để trống');

        const descrt = String(row['Mô tả'] || row['descrt'] || '').trim();
        const statusText = String(row['Trạng thái'] || row['status'] || '');
        const isActive = this._parseStatus(statusText);

        const specData = {
          specializationName,
          descrt,
          isActive,
        };

        const result = await this.createSpecialization(specData);
        results.success.push({ row: rowNumber, data: result });
      } catch (err) {
        results.errors.push({ row: rowNumber, data: row, error: err.message });
      }
    }

    return results;
  },

  _parseStatus(value) {
    if (value === undefined || value === null) return true;
    if (typeof value === 'boolean') return value;
    const str = String(value).toLowerCase().trim();
    if (str === 'đang hoạt động' || str === 'active' || str === 'true' || str === '1') return true;
    if (str === 'ngừng hoạt động' || str === 'inactive' || str === 'false' || str === '0') return false;
    return true;
  },
};

export default specializationService;