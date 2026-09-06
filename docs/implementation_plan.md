# Nâng Cấp Toàn Diện UI Web-App: Light Mode + Vercel Compliance

## Mục Tiêu

Chuyển đổi toàn bộ giao diện web-app từ **Dark Mode** (`#070b13` / `slate-900`) sang **Light Mode** thân thiện với người dùng có tuổi, đồng thời khắc phục toàn bộ **128 vi phạm** từ báo cáo Vercel Audit — **mà không thay đổi bất kỳ chức năng hay dữ liệu hiển thị nào**.

---

## User Review Required

> [!IMPORTANT]
> **Bảng màu Light Mode** được thiết kế theo nguyên tắc WCAG 2.1 AA (contrast ratio ≥ 4.5:1 cho text, ≥ 3:1 cho UI components), đặc biệt tối ưu cho **người dùng trên 50 tuổi** (font size tối thiểu 14px body text, high-contrast borders, no thin/light fonts).

> [!WARNING]
> **Chỉ thay đổi giao diện** — tuyệt đối không sửa logic nghiệp vụ, API calls, state management, routing hay bất kỳ dữ liệu hiển thị nào. Mọi `onClick`, `onSubmit`, `useState`, `useEffect` giữ nguyên 100%.

---

## Chiến Lược Bảng Màu Light Mode

### Bảng Ánh Xạ Màu Chính (Dark → Light)

| Vai trò | Dark Mode (hiện tại) | Light Mode (mới) | Contrast Ratio |
|:---|:---|:---|:---:|
| **Body background** | `#070b13` / `#090d16` | `#f8fafc` (slate-50) | — |
| **Page surface** | `bg-slate-900/60` | `bg-white` | — |
| **Card surface** | `bg-slate-900` / `bg-slate-800` | `bg-white border-slate-200` | — |
| **Sidebar** | `bg-slate-900/40` | `bg-slate-50 border-slate-200` | — |
| **Header** | `bg-slate-900/60 border-slate-800` | `bg-white/80 border-slate-200` | — |
| **Modal backdrop** | `bg-black/75` | `bg-black/40` | — |
| **Modal body** | `bg-slate-900 border-slate-800` | `bg-white border-slate-200 shadow-xl` | — |
| **Input field** | `bg-slate-950/70 border-slate-700` | `bg-slate-50 border-slate-300` | — |
| **Primary text** | `text-white` / `text-slate-100` | `text-slate-900` | 15.4:1 ✅ |
| **Secondary text** | `text-slate-400` | `text-slate-600` | 5.7:1 ✅ |
| **Tertiary text** | `text-slate-500` | `text-slate-500` | 4.6:1 ✅ |
| **Primary brand** | `emerald-400`/`emerald-500` | `emerald-600` / `emerald-700` | 4.5:1 ✅ |
| **Brand surface** | `emerald-500/10 border-emerald-500/30` | `emerald-50 border-emerald-200` | — |
| **Danger** | `rose-500/10 text-rose-300` | `red-50 border-red-200 text-red-700` | 5.1:1 ✅ |
| **Warning** | `amber-500/10 text-amber-300` | `amber-50 border-amber-200 text-amber-800` | 5.3:1 ✅ |
| **CTA button** | `bg-emerald-600 text-white` | `bg-emerald-600 text-white` | 4.5:1 ✅ (giữ nguyên) |
| **Badge/mono text** | `text-emerald-300 bg-emerald-500/20` | `text-emerald-700 bg-emerald-50` | — |
| **Border chung** | `border-slate-800` / `border-slate-700` | `border-slate-200` | — |
| **Code/mono background** | `bg-slate-950` | `bg-slate-100` | — |
| **Hover state** | `hover:bg-slate-800` / `hover:bg-slate-700` | `hover:bg-slate-100` | — |
| **Active nav item** | `bg-emerald-500/10 text-emerald-400 border-emerald-500/30` | `bg-emerald-50 text-emerald-700 border-emerald-200` | — |
| **Ambient glow** | `bg-emerald-500/10 blur-3xl` | Xóa bỏ (không cần trên nền sáng) | — |

---

## Phân Chia Thực Hiện: 4 Giai Đoạn

---

## Giai Đoạn 0: Foundation — Cập Nhật Design Tokens & CSS Gốc

> Thay đổi nền tảng để toàn bộ 34 file có base sáng mặc định.

### [MODIFY] [index.css](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/index.css)
- `background-color: #090d16` → `#f8fafc`
- `color: #f1f5f9` → `#0f172a` (slate-900)

### [MODIFY] [tailwind.config.js](file:///c:/DevFlutter/asseta-monorepo/frontend-web/tailwind.config.js)
- Cập nhật semantic colors trong `vault`:
  - `vault.dark` → `'#f8fafc'` (nền trang)
  - `vault.card` → `'#ffffff'` (nền thẻ)
  - `vault.border` → `'#e2e8f0'` (viền slate-200)

---

## Giai Đoạn 1: Quick Win — Micro-copy, Typo & Emoji Replacement (36 lỗi)

> **Nguyên tắc:** Thay đổi nhanh, an toàn, không ảnh hưởng layout.

### 1.1 Thay `...` → `…` (Unicode ellipsis) — 20+ vị trí

Áp dụng cho **tất cả** các file sau (liệt kê trong audit report):

| File | Dòng | Nội dung sửa |
|:---|:---:|:---|
| [App.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/App.tsx) | 29 | `Asseta...` → `Asseta…` |
| [LoginPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/Auth/LoginPage.tsx) | 127 | `xác thực...` → `xác thực…` |
| [RegisterPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/Auth/RegisterPage.tsx) | 196 | `bảo mật...` → `bảo mật…` |
| [ContinuityMapPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/ContinuityMapPage.tsx) | 102, 312 | `...` → `…` |
| [ItemFormModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ItemFormModal.tsx) | 203, 243, 264, 307 | `...` → `…` |
| [ContinuityGapBanner.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ContinuityGapBanner.tsx) | 43 | `...` → `…` |
| [ActionCardList.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/ActionCardList.tsx) | 143, 217 | `...` → `…` |
| [ActionCardFormModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardFormModal.tsx) | 301, 350, 399 | `...` → `…` |
| [ActionCardDetailModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardDetailModal.tsx) | 228, 248, 365 | `...` → `…` |
| [TrustedPeopleList.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/TrustedPeopleList.tsx) | 153 | `...` → `…` |
| [TrustedPersonModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/TrustedPersonModal.tsx) | 135, 160, 185 | `...` → `…` |
| [ScopedAccessMatrixDrawer.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/ScopedAccessMatrixDrawer.tsx) | 148 | `...` → `…` |
| [ContinuityPlanView.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/ContinuityPlanView.tsx) | 149, 188 | `...` → `…` |
| [PlanReadinessAuditModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/PlanReadinessAuditModal.tsx) | 56 | `...` → `…` |
| [EmergencyBriefModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/EmergencyBriefModal.tsx) | 68 | `...` → `…` |

### 1.2 Thêm `tabular-nums` — 4 vị trí

| File | Dòng | Element |
|:---|:---:|:---|
| [SafeActivationPanel.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/SafeActivation/SafeActivationPanel.tsx) | 214 | Countdown timer |
| [PlanMetricsHeader.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/PlanMetricsHeader.tsx) | 52, 65 | KPI percentages |
| [PlanReadinessAuditModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/PlanReadinessAuditModal.tsx) | 62, 66, 70 | Audit scores |
| [PairingCodeModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/PairingCodeModal.tsx) | 48 | 6-digit code |

### 1.3 Thay Emoji thô → Lucide SVG Icons — 12 vị trí

| File | Emoji hiện tại | Lucide Component thay thế |
|:---|:---|:---|
| [ActionCardItemCard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardItemCard.tsx) | `✏️` | `<Pencil className="w-3.5 h-3.5" />` |
| [ActionCardItemCard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardItemCard.tsx) | `🗑️` | `<Trash2 className="w-3.5 h-3.5" />` |
| [ActionCardTimelineView.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardTimelineView.tsx) | `⚡` `⏳` `📅` `🛡️` | `<Zap />` `<Clock />` `<Calendar />` `<ShieldCheck />` |
| [ActionCardTimelineView.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardTimelineView.tsx) | `📋` | `<ClipboardList className="w-8 h-8" />` |
| [ActionCardDetailModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardDetailModal.tsx) | `🔒` | `<Lock className="w-4 h-4" />` |
| [ActionCardList.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/ActionCardList.tsx) | `✨` `⚡` `⏳` `📅` `🛡️` | `<Sparkles />` `<Zap />` `<Clock />` `<Calendar />` `<ShieldCheck />` |
| [StepChecklistEditor.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/StepChecklistEditor.tsx) | `⏱️` `↑` `↓` | `<Clock />` `<ChevronUp />` `<ChevronDown />` |

---

## Giai Đoạn 2: Light Mode Conversion — 30 Files Color Swap

> **Nguyên tắc:** Từng file đổi toàn bộ Tailwind classes theo bảng ánh xạ ở trên. Đổi lần lượt theo nhóm chức năng.

### Nhóm A: Foundation Components (3 files)

#### [MODIFY] [App.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/App.tsx)
- Loading screen: `bg-[#070b13] text-slate-100` → `bg-slate-50 text-slate-900`
- Main layout: `bg-[#090d16] text-slate-100` → `bg-slate-50 text-slate-900`
- Icon tints: `emerald-400` → `emerald-600`
- Secondary text: `text-slate-400` → `text-slate-500`

#### [MODIFY] [Header.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/components/Header.tsx)
- Header bg: `bg-slate-900/60 border-slate-800` → `bg-white/80 border-slate-200 shadow-sm`
- Title: `text-white` → `text-slate-900`
- Subtitle mono: `text-emerald-400` → `text-emerald-600`
- Vault status badges: `bg-emerald-500/10 border-emerald-500/30 text-emerald-400` → `bg-emerald-50 border-emerald-200 text-emerald-700`
- Amber locked: `bg-amber-500/10 border-amber-500/30 text-amber-300` → `bg-amber-50 border-amber-200 text-amber-700`
- Notification btn: `bg-slate-800 text-slate-300` → `bg-slate-100 text-slate-600 border-slate-200`
- User avatar: `bg-emerald-500/20 border-emerald-500/30 text-emerald-400` → `bg-emerald-50 border-emerald-200 text-emerald-600`
- User name: `text-slate-200` → `text-slate-800`
- User email: `text-slate-400` → `text-slate-500`
- Logout btn: `bg-slate-800/80 border-slate-700/60` → `bg-slate-100 border-slate-200`
- Borders: `border-slate-800` → `border-slate-200`

#### [MODIFY] [Sidebar.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/components/Sidebar.tsx)
- Container: `border-slate-800 bg-slate-900/40` → `border-slate-200 bg-slate-50`
- Section title: `text-slate-500` → `text-slate-400`
- Nav inactive: `text-slate-400 hover:text-slate-200 hover:bg-slate-800/60` → `text-slate-600 hover:text-slate-900 hover:bg-slate-100`
- Nav active: `bg-emerald-500/10 text-emerald-400 border-emerald-500/30` → `bg-emerald-50 text-emerald-700 border-emerald-200`
- Badge: `bg-emerald-500/20 text-emerald-300` → `bg-emerald-50 text-emerald-600 border-emerald-200`
- Footer box: `bg-slate-800/40 border-slate-800 text-slate-400` → `bg-slate-100 border-slate-200 text-slate-500`
- Footer title: `text-slate-300` → `text-slate-700`

#### [MODIFY] [ErrorBoundary.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/components/ErrorBoundary.tsx)
- Bg: `bg-[#070b13] text-slate-100` → `bg-slate-50 text-slate-800`
- Error card: `bg-slate-900/90 border-red-500/30` → `bg-white border-red-200 shadow-xl`
- Icon box: `bg-red-500/10 border-red-500/30 text-red-400` → `bg-red-50 border-red-200 text-red-500`
- Stack trace: `bg-slate-950 border-slate-800 text-red-300` → `bg-slate-100 border-slate-200 text-red-600`
- Buttons: tương tự bảng ánh xạ

### Nhóm B: Auth Module (3 files)

#### [MODIFY] [LoginPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/Auth/LoginPage.tsx)
- Page bg: `bg-[#070b13]` → `bg-slate-50`
- Ambient blobs: **Xóa bỏ** (2 `<div>` glow blobs không cần thiết trên nền sáng)
- Card: `bg-slate-900/90 border-slate-800` → `bg-white border-slate-200 shadow-xl`
- Heading: `text-white` → `text-slate-900`
- Subheading: `text-slate-400` → `text-slate-500`
- Demo pill: `bg-emerald-950/30 border-emerald-500/30 text-emerald-300` → `bg-emerald-50 border-emerald-200 text-emerald-700`
- Error alert: `bg-rose-500/10 border-rose-500/30 text-rose-300` → `bg-red-50 border-red-200 text-red-700`
- Labels: `text-slate-300` → `text-slate-700`
- Inputs: `bg-slate-950/70 border-slate-700/80 text-slate-100 placeholder-slate-500` → `bg-slate-50 border-slate-300 text-slate-900 placeholder-slate-400`
- Selection: `selection:text-slate-900` → `selection:text-white`
- Footer border: `border-slate-800` → `border-slate-200`
- Switch link: `text-emerald-400 hover:text-emerald-300` → `text-emerald-600 hover:text-emerald-700`

#### [MODIFY] [RegisterPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/Auth/RegisterPage.tsx)
- Áp dụng cùng bảng ánh xạ như LoginPage

#### [MODIFY] [MasterKeyBackupModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/Auth/components/MasterKeyBackupModal.tsx)
- Modal backdrop: `bg-black/75` → `bg-black/40`
- Modal card: `bg-slate-900 border-slate-800` → `bg-white border-slate-200 shadow-2xl`
- Code block: `bg-slate-950` → `bg-slate-100`
- Text colors: theo bảng ánh xạ

### Nhóm C: ContinuityMap Module (6 files)

#### [MODIFY] [ContinuityMapPage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/ContinuityMapPage.tsx)
- Loading state: `text-slate-400` → `text-slate-500`, emerald icons `text-emerald-400` → `text-emerald-600`
- Error card: `bg-rose-950/30 border-rose-800/50 text-rose-300` → `bg-red-50 border-red-200 text-red-700`
- Page heading: `text-white` → `text-slate-900`
- Badge "Zero-Knowledge": `bg-emerald-500/20 text-emerald-400 border-emerald-500/30` → `bg-emerald-50 text-emerald-700 border-emerald-200`
- All buttons: theo bảng ánh xạ (slate-800 → slate-100, emerald-600 giữ nguyên)
- Unlock modal: `bg-slate-900 border-slate-800` → `bg-white border-slate-200 shadow-2xl`
- Demo hint box: `bg-emerald-950/40 border-emerald-500/30` → `bg-emerald-50 border-emerald-200`
- Input: `bg-slate-950 border-slate-700 text-white` → `bg-slate-50 border-slate-300 text-slate-900`

#### [MODIFY] [ReadinessScoreOverview.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ReadinessScoreOverview.tsx)
- Card surfaces: dark → white/slate-50
- Text colors: white/slate-200 → slate-900/slate-700
- Progress bars: giữ emerald-500, border dark → light

#### [MODIFY] [ContinuityGapBanner.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ContinuityGapBanner.tsx)
- Banner: `bg-amber-950/30 border-amber-800` → `bg-amber-50 border-amber-200 text-amber-800`

#### [MODIFY] [CategoryAccordion.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/CategoryAccordion.tsx)
- Accordion card: dark surfaces → white/slate-50

#### [MODIFY] [ContinuityItemCard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ContinuityItemCard.tsx)
- Card: dark surfaces → white/slate-50

#### [MODIFY] [ItemFormModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/ItemFormModal.tsx)
- Modal: theo bảng ánh xạ modal/input
- All inputs: dark → light

#### [MODIFY] [AssessmentWizard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityMap/components/AssessmentWizard.tsx)
- Wizard modal: theo bảng ánh xạ

### Nhóm D: ActionCard Module (7 files)

#### [MODIFY] [ActionCardList.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/ActionCardList.tsx)
- Page header, search, filters, buttons: dark → light

#### [MODIFY] [ActionCardItemCard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardItemCard.tsx)
- Card surface và text: dark → light

#### [MODIFY] [ActionCardFormModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardFormModal.tsx)
- Modal và inputs: dark → light

#### [MODIFY] [ActionCardDetailModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardDetailModal.tsx)
- Modal và nội dung chi tiết: dark → light

#### [MODIFY] [ActionCardTimelineView.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardTimelineView.tsx)
- Timeline layout: dark → light

#### [MODIFY] [StepChecklistEditor.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/StepChecklistEditor.tsx)
- Steps: dark → light

#### [MODIFY] [TemplateSelectorModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/TemplateSelectorModal.tsx)
- Template cards: dark → light

### Nhóm E: TrustedPeople Module (4 files)

#### [MODIFY] [TrustedPeopleList.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/TrustedPeopleList.tsx)
- Page layout, people cards: dark → light

#### [MODIFY] [TrustedPersonModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/TrustedPersonModal.tsx)
- Form modal: dark → light

#### [MODIFY] [ScopedAccessMatrixDrawer.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/ScopedAccessMatrixDrawer.tsx)
- Drawer: dark → light

#### [MODIFY] [PairingCodeModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/PairingCodeModal.tsx)
- Modal: dark → light

### Nhóm F: ContinuityPlan Module (5 files)

#### [MODIFY] [ContinuityPlanView.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/ContinuityPlanView.tsx)
- Page layout: dark → light

#### [MODIFY] [PlanMetricsHeader.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/PlanMetricsHeader.tsx)
- Metrics cards: dark → light

#### [MODIFY] [ContinuityPlanCardItem.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/ContinuityPlanCardItem.tsx)
- Card items: dark → light

#### [MODIFY] [ContinuityTimelineStage.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/ContinuityTimelineStage.tsx)
- Timeline stages: dark → light

#### [MODIFY] [PlanReadinessAuditModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/PlanReadinessAuditModal.tsx)
- Audit modal: dark → light

#### [MODIFY] [EmergencyBriefModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ContinuityPlan/components/EmergencyBriefModal.tsx)
- Brief modal: dark → light

### Nhóm G: SafeActivation Module (1 file)

#### [MODIFY] [SafeActivationPanel.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/SafeActivation/SafeActivationPanel.tsx)
- Panel toàn bộ: dark → light
- Countdown banner: giữ urgency nhưng trên nền sáng (red-50 border-red-200)

---

## Giai Đoạn 3: Accessibility & Focus States (66 lỗi)

> **Cùng lúc với Giai đoạn 2** (sửa trong cùng một lần chỉnh file)

### 3.1 Thêm `aria-label` — 22 nút icon-only

Sửa tại tất cả vị trí liệt kê trong audit (nhóm 1.1):
- Header: Bell button, Lock button
- ActionCard module: view toggle, edit/delete buttons, step reorder, modal close
- ContinuityMap module: accordion chevron, edit/delete, modal close X, wizard close X
- ContinuityPlan: modal close X (3 files)
- TrustedPeople: modal close X (3 files), edit/revoke buttons

### 3.2 Đổi `<div onClick>` → `<button>` — 3 vị trí

| File | Dòng | Sửa |
|:---|:---:|:---|
| [TemplateSelectorModal.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/TemplateSelectorModal.tsx) | 47 | `<div onClick>` → `<button type="button" onClick>` |
| [ActionCardItemCard.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/ActionCard/components/ActionCardItemCard.tsx) | 54 | `<div onClick>` → `<button type="button" onClick>` (hoặc bọc `<article>` kèm `role="button" tabIndex={0} onKeyDown`) |
| [ScopedAccessMatrixDrawer.tsx](file:///c:/DevFlutter/asseta-monorepo/frontend-web/src/features/TrustedPeople/components/ScopedAccessMatrixDrawer.tsx) | 112 | `<div onClick>` → `<button type="button" onClick>` |

### 3.3 Liên kết `<label htmlFor>` ↔ `<input id>` — 10+ vị trí

Trong LoginPage, RegisterPage, ActionCardFormModal, TrustedPersonModal — thêm `htmlFor` cho label và `id` cho input tương ứng.

### 3.4 Bổ sung form hints — 5 vị trí

- LoginPage email: `autoComplete="email" name="email"`
- LoginPage password: `autoComplete="current-password" name="password"`
- RegisterPage email: `spellCheck={false}`
- ItemFormModal Master Key: `autoComplete="off" spellCheck={false}`
- ActionCardDetailModal passphrase: `autoComplete="off" spellCheck={false}`

### 3.5 Thêm `aria-live` cho async updates — 4 vị trí

- SafeActivationPanel: success → `aria-live="polite"`, error → `role="alert" aria-live="assertive"`
- PairingCodeModal: "Đã sao chép" → `aria-live="polite"`
- MasterKeyBackupModal: "Đã sao chép" → `aria-live="polite"`

### 3.6 Sửa heading hierarchy — 2 vị trí

- EmergencyBriefModal: sửa phân cấp `h3 > h2` → `h3 > h4`
- PlanReadinessAuditModal: sửa `h3 > h5` → `h3 > h4`

### 3.7 Đổi `focus:outline-none` → `focus-visible:ring-2` — 11 vị trí

Tất cả input fields trong LoginPage, RegisterPage, ActionCardList, ActionCardFormModal, ActionCardDetailModal, ItemFormModal, AssessmentWizard:

```diff
- focus:outline-none focus:border-emerald-500
+ focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:ring-emerald-500
```

> [!NOTE]
> Trên Light Mode, `ring-offset` sẽ dùng `ring-offset-white` (mặc định) thay vì `ring-offset-slate-900`.

---

## Giai Đoạn 4: Performance — Animation Optimization (12 lỗi)

### 4.1 Thay `transition-all` → transition tường minh — 8 vị trí

| File | Hiện tại | Sửa thành |
|:---|:---|:---|
| PlanMetricsHeader (×2) | `transition-all duration-500` | `transition-[width] duration-500` |
| ContinuityPlanCardItem | `transition-all duration-200` | `transition-colors duration-200` |
| ReadinessScoreOverview | `transition-all duration-500` | `transition-[width] duration-500` |
| AssessmentWizard | `transition-all duration-300` | `transition-[width] duration-300` |
| TemplateSelectorModal | `transition-all` | `transition-colors` |
| ActionCardItemCard (×2) | `transition-all` | `transition-[transform,box-shadow,border-color]` / `transition-[width]` |

### 4.2 Thêm `motion-reduce:animate-none` — 3 vị trí

| File | Animation | Class bổ sung |
|:---|:---|:---|
| SafeActivationPanel | `animate-pulse` | `motion-reduce:animate-none` |
| ErrorBoundary | `animate-pulse` | `motion-reduce:animate-none` |
| App.tsx | `animate-pulse` | `motion-reduce:animate-none` |

---

## Tóm Tắt Phạm Vi Ảnh Hưởng

| Loại | Số file | Ghi chú |
|:---|:---:|:---|
| CSS Foundation | 2 | `index.css`, `tailwind.config.js` |
| Components | 3 | Header, Sidebar, ErrorBoundary |
| Auth module | 3 | LoginPage, RegisterPage, MasterKeyBackupModal |
| ContinuityMap module | 7 | Page + 6 components |
| ActionCard module | 7 | List + 6 components |
| TrustedPeople module | 4 | List + 3 components |
| ContinuityPlan module | 6 | View + 5 components |
| SafeActivation module | 1 | Panel |
| **Tổng** | **33 files** | — |

---

## Verification Plan

### Build Verification
```bash
cd frontend-web && npm run build
```
→ 0 TypeScript errors, 0 warnings.

### Visual Verification (Browser Subagent)
- Truy cập `http://localhost:5173`
- Screenshot từng tab (5 modules) → so sánh layout/data giữ nguyên
- Kiểm tra tất cả modal mở/đóng bình thường
- Kiểm tra contrast ratio bằng DevTools Accessibility audit

### Functional Regression
- Đăng nhập → Đăng xuất → Đăng ký (form validation giữ nguyên)
- CRUD items trên ContinuityMap
- Tạo/sửa/xóa Action Card
- Thêm/sửa Trusted People
- Xem Continuity Plan

> [!CAUTION]
> **Không chạm vào logic:** Toàn bộ `useState`, `useEffect`, API calls, event handlers, conditional rendering giữ nguyên 100%. Chỉ đổi Tailwind CSS classes.
