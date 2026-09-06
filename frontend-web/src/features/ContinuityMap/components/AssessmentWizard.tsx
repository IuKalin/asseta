import React, { useState } from 'react';
import { AlertCircle, ArrowLeft, ArrowRight, CheckCircle2, HelpCircle, Shield, Sparkles, X } from 'lucide-react';
import { AssessmentAnswer, PriorityLevel } from '../../../types/continuity';

interface QuestionDef {
  id: string;
  categoryCode: string;
  categoryTitle: string;
  itemName: string;
  prompt: string;
  hintSuggestion: string;
  defaultPriority: PriorityLevel;
}

const QUESTIONS: QuestionDef[] = [
  {
    id: 'Q1',
    categoryCode: 'FINANCIAL',
    categoryTitle: 'Tài chính & Ngân hàng',
    itemName: 'Tài khoản thanh toán & Sổ tiết kiệm',
    prompt: 'Bạn có tài khoản ngân hàng chính hoặc sổ tiết kiệm cần người thân nắm bắt?',
    hintSuggestion: 'App ngân hàng điện tử / Sổ cứng tại két sắt',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q2',
    categoryCode: 'FINANCIAL',
    categoryTitle: 'Tài chính & Nghĩa vụ tiền tệ',
    itemName: 'Khoản vay & Nghĩa vụ trả nợ',
    prompt: 'Bạn có khoản vay thế chấp, tín chấp hoặc trả góp cần thanh toán định kỳ?',
    hintSuggestion: 'Hợp đồng tín dụng lưu tại tủ hồ sơ',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q3',
    categoryCode: 'PROPERTY',
    categoryTitle: 'Bất động sản',
    itemName: 'Nhà ở & Bất động sản sở hữu',
    prompt: 'Bạn có sở hữu nhà đất, chung cư hoặc hợp đồng thuê dài hạn?',
    hintSuggestion: 'Sổ đỏ lưu két sắt / Hợp đồng công chứng',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q4',
    categoryCode: 'PROPERTY',
    categoryTitle: 'Tài sản vật lý',
    itemName: 'Phương tiện đi lại (Ô tô / Xe máy)',
    prompt: 'Bạn có xe ô tô hoặc phương tiện chính đứng tên sở hữu cá nhân?',
    hintSuggestion: 'Đăng ký xe bản gốc lưu trong ví / ngăn kéo xe',
    defaultPriority: 'IMPORTANT',
  },
  {
    id: 'Q5',
    categoryCode: 'INSURANCE',
    categoryTitle: 'Bảo hiểm',
    itemName: 'Hợp đồng bảo hiểm nhân thọ',
    prompt: 'Bạn có tham gia hợp đồng bảo hiểm nhân thọ để bảo vệ quyền lợi gia đình?',
    hintSuggestion: 'Hợp đồng điện tử trong email / bản in tại tủ',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q6',
    categoryCode: 'INSURANCE',
    categoryTitle: 'Bảo hiểm y tế',
    itemName: 'Thẻ bảo hiểm sức khỏe cao cấp',
    prompt: 'Bạn có thẻ bảo hiểm sức khỏe tư nhân hỗ trợ thanh toán viện phí?',
    hintSuggestion: 'Thẻ cứng để trong ví cá nhân / ảnh chụp điện thoại',
    defaultPriority: 'IMPORTANT',
  },
  {
    id: 'Q7',
    categoryCode: 'BUSINESS',
    categoryTitle: 'Doanh nghiệp',
    itemName: 'Cổ phần & Vốn góp công ty',
    prompt: 'Bạn có cổ phần doanh nghiệp, tư cách đại diện pháp luật hoặc góp vốn kinh doanh?',
    hintSuggestion: 'Giấy chứng nhận ĐKKD / Điều lệ công ty',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q8',
    categoryCode: 'BUSINESS',
    categoryTitle: 'Quan hệ đối tác',
    itemName: 'Thỏa thuận hợp tác đối tác then chốt',
    prompt: 'Có hợp đồng đối tác hoặc nghĩa vụ thương quyền nào cần bàn giao người kế nhiệm?',
    hintSuggestion: 'Thư mục văn bản pháp lý lưu tại công ty',
    defaultPriority: 'IMPORTANT',
  },
  {
    id: 'Q9',
    categoryCode: 'DOCUMENTS',
    categoryTitle: 'Hồ sơ pháp lý',
    itemName: 'Giấy tờ tùy thân & Hộ chiếu bản gốc',
    prompt: 'Vị trí cất giữ hộ chiếu, căn cước công dân và sổ hộ khẩu đã được sắp xếp rõ ràng?',
    hintSuggestion: 'Tủ tài liệu phòng ngủ ngăn số 1',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q10',
    categoryCode: 'DOCUMENTS',
    categoryTitle: 'Hồ sơ thừa kế',
    itemName: 'Di chúc / Thỏa thuận phân chia',
    prompt: 'Bạn đã từng lập văn bản di chúc hoặc nguyện vọng tiếp quản tài sản cá nhân chưa?',
    hintSuggestion: 'Bản lưu tại văn phòng luật sư / két sắt',
    defaultPriority: 'IMPORTANT',
  },
  {
    id: 'Q11',
    categoryCode: 'FAMILY',
    categoryTitle: 'Gia đình',
    itemName: 'Nghĩa vụ chu cấp & Phụng dưỡng',
    prompt: 'Bạn có nghĩa vụ chu cấp học phí con cái hoặc phụng dưỡng cha mẹ hàng tháng?',
    hintSuggestion: 'Lịch chuyển tiền tự động / ghi chú ngân sách',
    defaultPriority: 'CRITICAL',
  },
  {
    id: 'Q12',
    categoryCode: 'FAMILY',
    categoryTitle: 'Liên lạc khẩn cấp',
    itemName: 'Danh sách người thân cận tiếp quản',
    prompt: 'Đã có danh sách số điện thoại của người thân và luật sư đáng tin cậy chưa?',
    hintSuggestion: 'Ghi chú khẩn cấp lưu trên điện thoại',
    defaultPriority: 'IMPORTANT',
  },
];

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (answers: AssessmentAnswer[]) => Promise<void>;
}

export const AssessmentWizard: React.FC<Props> = ({ isOpen, onClose, onSubmit }) => {
  const [currentIndex, setCurrentIndex] = useState<number>(0);
  const [answers, setAnswers] = useState<Record<string, { hasItem: boolean; hint: string; priority: PriorityLevel }>>(() => {
    const initial: Record<string, { hasItem: boolean; hint: string; priority: PriorityLevel }> = {};
    QUESTIONS.forEach((q) => {
      initial[q.id] = { hasItem: true, hint: '', priority: q.defaultPriority };
    });
    return initial;
  });

  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  if (!isOpen) return null;

  const currentQ = QUESTIONS[currentIndex];
  const currentAnswer = answers[currentQ.id];

  const handleToggleHasItem = (has: boolean) => {
    setAnswers((prev) => ({
      ...prev,
      [currentQ.id]: {
        ...prev[currentQ.id],
        hasItem: has,
      },
    }));
  };

  const handleUpdateHint = (hintText: string) => {
    setAnswers((prev) => ({
      ...prev,
      [currentQ.id]: {
        ...prev[currentQ.id],
        hint: hintText,
      },
    }));
  };

  const handleNext = () => {
    if (currentIndex < QUESTIONS.length - 1) {
      setCurrentIndex((prev) => prev + 1);
    }
  };

  const handlePrev = () => {
    if (currentIndex > 0) {
      setCurrentIndex((prev) => prev - 1);
    }
  };

  const handleFinish = async () => {
    try {
      setIsSubmitting(true);
      setSubmitError(null);
      const answerList: AssessmentAnswer[] = QUESTIONS.map((q) => ({
        questionId: q.id,
        categoryCode: q.categoryCode,
        itemName: q.itemName,
        hasItem: answers[q.id].hasItem,
        priority: answers[q.id].priority,
        documentLocationHint: answers[q.id].hint.trim() || undefined,
      }));

      await onSubmit(answerList);
      onClose();
    } catch (err: any) {
      console.error('Failed to submit assessment:', err);
      const msg =
        err.response?.data?.message ||
        err.response?.data?.title ||
        err.message ||
        'Có lỗi xảy ra khi hoàn tất khảo sát.';
      setSubmitError(msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  const completedCount = Object.values(answers).filter((a) => a.hasItem).length;
  const progressPercent = Math.round(((currentIndex + 1) / QUESTIONS.length) * 100);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm animate-in fade-in">
      <div className="bg-white border border-slate-200 rounded-3xl max-w-xl w-full p-6 sm:p-8 shadow-2xl space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="p-2 rounded-xl bg-emerald-50 text-emerald-600">
              <Sparkles className="w-5 h-5" />
            </div>
            <div>
              <h3 className="text-lg font-bold text-slate-900">Khảo Sát Tiếp Quản Nhanh</h3>
              <p className="text-xs text-slate-500">Thiết lập bản đồ tiếp tục cho tài sản & nghĩa vụ</p>
            </div>
          </div>
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng"
            className="p-2 rounded-lg text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Error Alert */}
        {submitError && (
          <div role="alert" className="flex items-center gap-2 p-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-xs">
            <AlertCircle className="w-4 h-4 shrink-0 text-red-500" />
            <span>{submitError}</span>
          </div>
        )}

        {/* Progress bar */}
        <div className="space-y-1.5">
          <div className="flex justify-between text-xs font-mono text-slate-500">
            <span>
              Câu hỏi <span className="tabular-nums">{currentIndex + 1}</span> / <span className="tabular-nums">{QUESTIONS.length}</span>
            </span>
            <span className="tabular-nums">{progressPercent}% hoàn thành</span>
          </div>
          <div className="w-full h-2 bg-slate-100 rounded-full overflow-hidden">
            <div
              className="h-full bg-emerald-600 transition-[width] duration-300 rounded-full"
              style={{ width: `${progressPercent}%` }}
            />
          </div>
        </div>

        {/* Question Card */}
        <div className="bg-slate-50 border border-slate-200 rounded-2xl p-6 space-y-4">
          <div className="flex items-center justify-between">
            <span className="text-[10px] font-mono uppercase tracking-wider px-2.5 py-1 rounded-full bg-white border border-slate-200 text-emerald-700 font-bold">
              {currentQ.categoryTitle}
            </span>
            <span className="text-xs font-semibold text-slate-500">{currentQ.itemName}</span>
          </div>

          <h4 className="text-base font-bold text-slate-900 leading-relaxed">{currentQ.prompt}</h4>

          {/* Yes / No selector */}
          <div className="grid grid-cols-2 gap-3 pt-2">
            <button
              type="button"
              onClick={() => handleToggleHasItem(true)}
              className={`py-3 px-4 rounded-xl font-bold text-xs border transition flex items-center justify-center gap-2 ${currentAnswer.hasItem
                  ? 'bg-emerald-50 border-emerald-300 text-emerald-800 shadow-sm'
                  : 'bg-white border-slate-200 text-slate-600 hover:border-slate-300'
                }`}
            >
              <CheckCircle2 className="w-4 h-4 text-emerald-600" /> Có tài sản / nghĩa vụ này
            </button>
            <button
              type="button"
              onClick={() => handleToggleHasItem(false)}
              className={`py-3 px-4 rounded-xl font-bold text-xs border transition flex items-center justify-center gap-2 ${!currentAnswer.hasItem
                  ? 'bg-slate-200 border-slate-300 text-slate-800 shadow-sm'
                  : 'bg-white border-slate-200 text-slate-500 hover:border-slate-300'
                }`}
            >
              Không áp dụng
            </button>
          </div>

          {/* Hint input if Yes */}
          {currentAnswer.hasItem && (
            <div className="space-y-1.5 pt-2 animate-in fade-in text-xs">
              <label htmlFor="wizardHintInput" className="text-slate-700 font-medium flex items-center gap-1.5">
                <HelpCircle className="w-3.5 h-3.5 text-emerald-600" />
                Vị trí lưu trữ hoặc gợi ý tìm hồ sơ (Không nhập mật khẩu):
              </label>
              <input
                id="wizardHintInput"
                type="text"
                placeholder={`Gợi ý: ${currentQ.hintSuggestion}`}
                value={currentAnswer.hint}
                onChange={(e) => handleUpdateHint(e.target.value)}
                className="w-full bg-white border border-slate-300 rounded-lg px-3 py-2 text-slate-900 placeholder-slate-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500 focus-visible:ring-offset-2 text-xs"
              />
            </div>
          )}
        </div>

        {/* Footer Navigation */}
        <div className="flex items-center justify-between pt-2">
          <button
            type="button"
            onClick={handlePrev}
            disabled={currentIndex === 0}
            className="flex items-center gap-1.5 px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 hover:text-slate-900 disabled:opacity-30 disabled:hover:text-slate-600 transition"
          >
            <ArrowLeft className="w-4 h-4" /> Quay lại
          </button>

          {currentIndex < QUESTIONS.length - 1 ? (
            <button
              type="button"
              onClick={handleNext}
              className="flex items-center gap-1.5 px-5 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-sm transition"
            >
              Tiếp theo <ArrowRight className="w-4 h-4" />
            </button>
          ) : (
            <button
              type="button"
              onClick={handleFinish}
              disabled={isSubmitting}
              className="flex items-center gap-2 px-6 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-bold shadow-sm transition disabled:opacity-50"
            >
              <Shield className="w-4 h-4" />
              {isSubmitting ? 'Đang tạo bản đồ tiếp quản…' : `Hoàn tất (${completedCount} mục)`}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};
