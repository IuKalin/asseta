enum UrgencyStage { immediate, first72Hours, first7Days, longerTerm }

extension UrgencyStageX on UrgencyStage {
  String toShortString() {
    switch (this) {
      case UrgencyStage.immediate:
        return 'IMMEDIATE';
      case UrgencyStage.first72Hours:
        return 'FIRST_72_HOURS';
      case UrgencyStage.first7Days:
        return 'FIRST_7_DAYS';
      case UrgencyStage.longerTerm:
        return 'LONGER_TERM';
    }
  }

  String get displayNameVi {
    switch (this) {
      case UrgencyStage.immediate:
        return 'Ngay lập tức';
      case UrgencyStage.first72Hours:
        return '72 Giờ đầu';
      case UrgencyStage.first7Days:
        return '7 Ngày đầu';
      case UrgencyStage.longerTerm:
        return 'Dài hạn';
    }
  }

  String get timeframeVi {
    switch (this) {
      case UrgencyStage.immediate:
        return '0 - 24 giờ đầu tiên';
      case UrgencyStage.first72Hours:
        return 'Ngày 1 - 3 sau sự kiện';
      case UrgencyStage.first7Days:
        return 'Tuần đầu tiên';
      case UrgencyStage.longerTerm:
        return 'Sau tuần đầu tiên';
    }
  }

  static UrgencyStage fromString(String val) {
    switch (val.toUpperCase()) {
      case 'IMMEDIATE':
        return UrgencyStage.immediate;
      case 'FIRST_7_DAYS':
        return UrgencyStage.first7Days;
      case 'LONGER_TERM':
        return UrgencyStage.longerTerm;
      case 'FIRST_72_HOURS':
      default:
        return UrgencyStage.first72Hours;
    }
  }
}
