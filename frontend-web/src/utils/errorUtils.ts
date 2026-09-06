/**
 * Trích xuất thông báo lỗi an toàn từ response backend API hoặc Exception.
 * Đảm bảo 100% luôn trả về chuỗi string, ngăn chặn lỗi render Object vào JSX của React 18.
 */
export function getErrorMessage(err: any, fallback: string = 'Đã xảy ra lỗi không xác định'): string {
  if (!err) return fallback;
  if (typeof err === 'string') return err;

  // Trường hợp backend trả về ApiResponse envelope với error object: { code, message, details }
  if (err.response?.data?.error?.message && typeof err.response.data.error.message === 'string') {
    return err.response.data.error.message;
  }

  // Trường hợp error là string trực tiếp
  if (typeof err.response?.data?.error === 'string') {
    return err.response.data.error;
  }

  // Trường hợp backend trả về message cấp cao nhất
  if (err.response?.data?.message && typeof err.response.data.message === 'string') {
    return err.response.data.message;
  }

  // Trường hợp Axios Error hoặc Error chuẩn của JS
  if (err.message && typeof err.message === 'string') {
    return err.message;
  }

  return fallback;
}
