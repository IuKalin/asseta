import React, { useState } from 'react';
import { Clock, ChevronUp, ChevronDown, X } from 'lucide-react';
import { ActionCardStep } from '../../../types/actionCard';

interface StepChecklistEditorProps {
  steps: ActionCardStep[];
  onToggleStep?: (stepId: string, isCompleted: boolean) => Promise<void>;
  onAddStep?: (instruction: string, estimatedDuration?: string) => Promise<void>;
  onDeleteStep?: (stepId: string) => Promise<void>;
  onReorderSteps?: (orderedStepIds: string[]) => Promise<void>;
  readOnly?: boolean;
}

export const StepChecklistEditor: React.FC<StepChecklistEditorProps> = ({
  steps,
  onToggleStep,
  onAddStep,
  onDeleteStep,
  onReorderSteps,
  readOnly = false,
}) => {
  const [newInstruction, setNewInstruction] = useState('');
  const [newDuration, setNewDuration] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const sortedSteps = [...steps].sort((a, b) => a.stepOrder - b.stepOrder);
  const isMaxStepsReached = sortedSteps.length >= 20;

  const handleAddStep = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newInstruction.trim() || isMaxStepsReached || !onAddStep) return;

    setIsSubmitting(true);
    try {
      await onAddStep(newInstruction.trim(), newDuration.trim() || undefined);
      setNewInstruction('');
      setNewDuration('');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleMoveUp = async (index: number) => {
    if (index === 0 || !onReorderSteps) return;
    const newOrdered = [...sortedSteps];
    const temp = newOrdered[index - 1];
    newOrdered[index - 1] = newOrdered[index];
    newOrdered[index] = temp;
    await onReorderSteps(newOrdered.map((s) => s.id));
  };

  const handleMoveDown = async (index: number) => {
    if (index === sortedSteps.length - 1 || !onReorderSteps) return;
    const newOrdered = [...sortedSteps];
    const temp = newOrdered[index + 1];
    newOrdered[index + 1] = newOrdered[index];
    newOrdered[index] = temp;
    await onReorderSteps(newOrdered.map((s) => s.id));
  };

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <h4 className="text-sm font-semibold text-slate-900">
          Các bước hành động ({sortedSteps.length}/20)
        </h4>
        {isMaxStepsReached && (
          <span className="text-xs text-amber-600 font-medium">
            Đã đạt giới hạn tối đa 20 bước
          </span>
        )}
      </div>

      {/* Steps List */}
      <div className="space-y-2">
        {sortedSteps.length === 0 ? (
          <p className="rounded-lg border border-dashed border-slate-200 bg-slate-50 p-4 text-center text-xs text-slate-500">
            Chưa có bước hành động nào được thiết lập.
          </p>
        ) : (
          sortedSteps.map((step, index) => (
            <div
              key={step.id}
              className={`flex items-center gap-3 rounded-lg border p-3 transition-colors ${step.isCompleted
                  ? 'border-emerald-200 bg-emerald-50/50'
                  : 'border-slate-200 bg-white shadow-xs'
                }`}
            >
              {/* Checkbox */}
              <input
                type="checkbox"
                checked={step.isCompleted}
                disabled={readOnly || !onToggleStep}
                onChange={(e) => onToggleStep && onToggleStep(step.id, e.target.checked)}
                className="h-4 w-4 rounded border-slate-300 bg-white text-emerald-600 focus:ring-emerald-500 focus:ring-offset-white"
              />

              {/* Order Badge */}
              <span className="flex h-5 w-5 items-center justify-center rounded-full bg-slate-100 text-[11px] font-semibold text-slate-600 tabular-nums">
                {step.stepOrder}
              </span>

              {/* Content */}
              <div className="flex-1">
                <p
                  className={`text-sm ${step.isCompleted
                      ? 'text-slate-400 line-through'
                      : 'text-slate-900 font-medium'
                    }`}
                >
                  {step.instruction}
                </p>
                {step.estimatedDuration && (
                  <span className="inline-flex items-center gap-1 mt-0.5 text-xs text-slate-500">
                    <Clock className="h-3 w-3 text-slate-400" aria-hidden="true" />
                    {step.estimatedDuration}
                  </span>
                )}
              </div>

              {/* Controls */}
              {!readOnly && (
                <div className="flex items-center gap-1">
                  {onReorderSteps && (
                    <>
                      <button
                        type="button"
                        disabled={index === 0}
                        onClick={() => handleMoveUp(index)}
                        className="rounded p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-700 disabled:opacity-20 transition-colors"
                        title="Di chuyển lên"
                        aria-label="Di chuyển lên"
                      >
                        <ChevronUp className="h-4 w-4" />
                      </button>
                      <button
                        type="button"
                        disabled={index === sortedSteps.length - 1}
                        onClick={() => handleMoveDown(index)}
                        className="rounded p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-700 disabled:opacity-20 transition-colors"
                        title="Di chuyển xuống"
                        aria-label="Di chuyển xuống"
                      >
                        <ChevronDown className="h-4 w-4" />
                      </button>
                    </>
                  )}
                  {onDeleteStep && (
                    <button
                      type="button"
                      onClick={() => onDeleteStep(step.id)}
                      className="rounded p-1 text-red-500 hover:bg-red-50 hover:text-red-700 transition-colors"
                      title="Xóa bước này"
                      aria-label="Xóa bước này"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  )}
                </div>
              )}
            </div>
          ))
        )}
      </div>

      {/* Add Step Form */}
      {!readOnly && onAddStep && !isMaxStepsReached && (
        <form onSubmit={handleAddStep} className="flex flex-col gap-2 rounded-lg border border-slate-200 bg-slate-50 p-3 sm:flex-row">
          <input
            type="text"
            placeholder="Nội dung bước hành động mới..."
            value={newInstruction}
            onChange={(e) => setNewInstruction(e.target.value)}
            maxLength={500}
            className="flex-1 rounded-md border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
          />
          <input
            type="text"
            placeholder="Thời gian (VD: 30 phút)"
            value={newDuration}
            onChange={(e) => setNewDuration(e.target.value)}
            maxLength={50}
            className="w-full rounded-md border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500 sm:w-36"
          />
          <button
            type="submit"
            disabled={!newInstruction.trim() || isSubmitting}
            className="rounded-md bg-emerald-600 px-4 py-1.5 text-xs font-semibold text-white hover:bg-emerald-500 disabled:opacity-50 transition-colors shadow-xs"
          >
            {isSubmitting ? 'Đang thêm…' : '+ Thêm bước'}
          </button>
        </form>
      )}
    </div>
  );
};
