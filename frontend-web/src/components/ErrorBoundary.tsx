import { Component, ErrorInfo, ReactNode } from 'react';
import { AlertTriangle, RefreshCw, Home } from 'lucide-react';

interface Props {
  children: ReactNode;
  fallbackTitle?: string;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
}

export class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: null,
    errorInfo: null,
  };

  public static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
      errorInfo: null,
    };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('[Asseta ErrorBoundary Caught Exception]:', error, errorInfo);
    this.setState({
      error,
      errorInfo,
    });
  }

  private handleReload = () => {
    window.location.reload();
  };

  private handleReset = () => {
    this.setState({ hasError: false, error: null, errorInfo: null });
  };

  public render() {
    if (this.state.hasError) {
      return (
        <div className="min-h-screen bg-slate-50 flex flex-col items-center justify-center p-6 text-slate-900">
          <div className="w-full max-w-lg rounded-2xl border border-red-200 bg-white p-8 shadow-xl flex flex-col items-center text-center">
            <div className="w-16 h-16 rounded-2xl bg-red-50 border border-red-200 flex items-center justify-center text-red-600 mb-6 animate-pulse motion-reduce:animate-none">
              <AlertTriangle className="w-8 h-8" />
            </div>

            <h2 className="text-xl font-bold text-slate-900 mb-2">
              {this.props.fallbackTitle || 'Đã xảy ra lỗi giao diện'}
            </h2>
            <p className="text-sm text-slate-600 mb-6">
              Hệ thống Asseta đã tự động bảo vệ dữ liệu và cách ly lỗi an toàn nhằm tránh tình trạng mất mát phiên làm việc.
            </p>

            {this.state.error && (
              <div className="w-full mb-6 rounded-xl border border-slate-200 bg-slate-100 p-4 text-left overflow-x-auto max-h-40 text-xs font-mono text-red-600">
                <p className="font-bold text-red-700 mb-1">{this.state.error.name}: {this.state.error.message}</p>
                {this.state.error.stack && (
                  <pre className="text-[10px] text-slate-600 whitespace-pre-wrap">{this.state.error.stack}</pre>
                )}
              </div>
            )}

            <div className="flex items-center gap-3 w-full sm:w-auto">
              <button
                type="button"
                onClick={this.handleReset}
                className="flex-1 sm:flex-initial flex items-center justify-center gap-2 rounded-xl border border-slate-200 bg-slate-100 px-5 py-2.5 text-xs font-semibold text-slate-700 hover:bg-slate-200 transition shadow-sm"
              >
                <Home className="w-4 h-4" />
                <span>Thử Lại Cục Bộ</span>
              </button>
              <button
                type="button"
                onClick={this.handleReload}
                className="flex-1 sm:flex-initial flex items-center justify-center gap-2 rounded-xl bg-red-600 px-5 py-2.5 text-xs font-semibold text-white hover:bg-red-700 transition shadow-sm"
              >
                <RefreshCw className="w-4 h-4" />
                <span>Tải Lại Trang</span>
              </button>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
