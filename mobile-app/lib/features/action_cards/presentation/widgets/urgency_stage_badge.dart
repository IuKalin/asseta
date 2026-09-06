import 'package:flutter/material.dart';
import '../../domain/entities/action_card_enums.dart';

class UrgencyStageBadge extends StatelessWidget {
  final UrgencyStage stage;

  const UrgencyStageBadge({Key? key, required this.stage}) : super(key: key);

  Color _getBackgroundColor() {
    switch (stage) {
      case UrgencyStage.immediate:
        return const Color(0x33EF4444); // Red 500 / 20%
      case UrgencyStage.first72Hours:
        return const Color(0x33F59E0B); // Amber 500 / 20%
      case UrgencyStage.first7Days:
        return const Color(0x3306B6D4); // Cyan 500 / 20%
      case UrgencyStage.longerTerm:
        return const Color(0x33A855F7); // Purple 500 / 20%
    }
  }

  Color _getTextColor() {
    switch (stage) {
      case UrgencyStage.immediate:
        return const Color(0xFFF87171); // Red 400
      case UrgencyStage.first72Hours:
        return const Color(0xFFFBBF24); // Amber 400
      case UrgencyStage.first7Days:
        return const Color(0xFF22D3EE); // Cyan 400
      case UrgencyStage.longerTerm:
        return const Color(0xFFC084FC); // Purple 400
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: _getBackgroundColor(),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        stage.displayNameVi,
        style: TextStyle(
          color: _getTextColor(),
          fontSize: 11,
          fontWeight: FontWeight.w600,
        ),
      ),
    );
  }
}
