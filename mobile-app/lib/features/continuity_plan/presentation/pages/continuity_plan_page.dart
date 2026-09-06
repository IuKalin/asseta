import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/continuity_plan_entity.dart';
import '../bloc/continuity_plan_bloc.dart';
import '../bloc/continuity_plan_event.dart';
import '../bloc/continuity_plan_state.dart';
import '../widgets/gap_alert_banner.dart';
import '../widgets/stage_timeline_tile.dart';
import 'emergency_brief_page.dart';

class ContinuityPlanPage extends StatefulWidget {
  const ContinuityPlanPage({super.key});

  @override
  State<ContinuityPlanPage> createState() => _ContinuityPlanPageState();
}

class _ContinuityPlanPageState extends State<ContinuityPlanPage> {
  @override
  void initState() {
    super.initState();
    context.read<ContinuityPlanBloc>().add(const LoadContinuityPlanEvent());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Kế Hoạch Kế Thừa (Continuity Plan)'),
        actions: [
          BlocBuilder<ContinuityPlanBloc, ContinuityPlanState>(
            builder: (context, state) {
              if (state is! ContinuityPlanLoadedState) return const SizedBox.shrink();
              return IconButton(
                icon: const Icon(Icons.menu_book),
                tooltip: 'Sổ tay khẩn cấp',
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => EmergencyBriefPage(plan: state.plan),
                    ),
                  );
                },
              );
            },
          ),
        ],
      ),
      body: BlocConsumer<ContinuityPlanBloc, ContinuityPlanState>(
        listener: (context, state) {
          if (state is ContinuityPlanLoadedState && state.actionMessage != null) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.actionMessage!),
                duration: const Duration(seconds: 3),
              ),
            );
          }
        },
        builder: (context, state) {
          if (state is ContinuityPlanLoadingState) {
            return const Center(child: CircularProgressIndicator());
          }

          if (state is ContinuityPlanErrorState) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline, color: Colors.red, size: 48),
                    const SizedBox(height: 12),
                    Text(
                      state.message,
                      textAlign: TextAlign.center,
                      style: const TextStyle(fontSize: 16),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      onPressed: () {
                        context.read<ContinuityPlanBloc>().add(const LoadContinuityPlanEvent());
                      },
                      icon: const Icon(Icons.refresh),
                      label: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is ContinuityPlanLoadedState) {
            return RefreshIndicator(
              onRefresh: () async {
                if (state.isDelegatedView) {
                  context.read<ContinuityPlanBloc>().add(const LoadMyDelegatedPlanEvent());
                } else {
                  context.read<ContinuityPlanBloc>().add(const LoadContinuityPlanEvent());
                }
              },
              child: ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                children: [
                  // View Switcher (Owner vs Delegated)
                  Padding(
                    padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
                    child: SegmentedButton<bool>(
                      segments: const [
                        ButtonSegment<bool>(
                          value: false,
                          label: Text('Kế hoạch của tôi'),
                          icon: Icon(Icons.person),
                        ),
                        ButtonSegment<bool>(
                          value: true,
                          label: Text('Được ủy quyền'),
                          icon: Icon(Icons.how_to_reg),
                        ),
                      ],
                      selected: {state.isDelegatedView},
                      onSelectionChanged: (Set<bool> selected) {
                        final isDelegated = selected.first;
                        if (isDelegated) {
                          context.read<ContinuityPlanBloc>().add(const LoadMyDelegatedPlanEvent());
                        } else {
                          context.read<ContinuityPlanBloc>().add(const LoadContinuityPlanEvent());
                        }
                      },
                    ),
                  ),

                  // Readiness Scorecard
                  _buildReadinessCard(context, state.plan),

                  // SPoF and Gap Alerts
                  GapAlertBanner(
                    hasSpofRisk: state.plan.hasSinglePointOfFailureRisk,
                    spofWarning: state.plan.singlePointOfFailureWarning,
                    gapsCount: state.plan.gapsCount,
                  ),

                  // Category & Gaps Filters
                  _buildFilterBar(context, state),

                  // Stages Timeline Tiles
                  ...state.filteredStages.map(
                    (stage) => StageTimelineTile(
                      stage: stage,
                      isDelegatedView: state.isDelegatedView,
                      onToggleCompletion: (cardId, rowVersion) {
                        context.read<ContinuityPlanBloc>().add(
                              ToggleCardCompletionEvent(
                                cardId: cardId,
                                rowVersion: rowVersion,
                              ),
                            );
                      },
                      onMoveStage: (cardId, newStage, rowVersion) {
                        context.read<ContinuityPlanBloc>().add(
                              UpdateCardStageEvent(
                                cardId: cardId,
                                newStage: newStage,
                                rowVersion: rowVersion,
                              ),
                            );
                      },
                    ),
                  ),
                  const SizedBox(height: 24),
                ],
              ),
            );
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildReadinessCard(BuildContext context, ContinuityPlanEntity plan) {
    Color scoreColor = Colors.teal;
    if (plan.overallPlanReadinessScore < 50) {
      scoreColor = Colors.red;
    } else if (plan.overallPlanReadinessScore < 80) {
      scoreColor = Colors.orange;
    }

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Điểm Sẵn Sàng Kế Thừa',
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 16,
                          color: Colors.black87,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Đo lường độ bao phủ người ủy thác & tính sẵn sàng vị trí hồ sơ',
                        style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
                  decoration: BoxDecoration(
                    color: scoreColor.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: scoreColor, width: 2),
                  ),
                  child: Text(
                    '${plan.overallPlanReadinessScore}%',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                      color: scoreColor,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                value: (plan.overallPlanReadinessScore / 100).clamp(0.0, 1.0),
                minHeight: 8,
                backgroundColor: Colors.grey.shade200,
                color: scoreColor,
              ),
            ),
            const SizedBox(height: 14),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _buildMetricSubItem(
                  'Ủy thác',
                  '${plan.delegateCoveragePercentage.toStringAsFixed(0)}%',
                  Icons.people_alt_outlined,
                  Colors.teal,
                ),
                _buildMetricSubItem(
                  'Hồ sơ',
                  '${plan.documentReadinessPercentage.toStringAsFixed(0)}%',
                  Icons.folder_shared_outlined,
                  Colors.indigo,
                ),
                _buildMetricSubItem(
                  'Tiến độ',
                  '${plan.completedCardsCount}/${plan.totalCardsCount}',
                  Icons.task_alt,
                  Colors.blueGrey,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMetricSubItem(String label, String value, IconData icon, Color color) {
    return Row(
      children: [
        Icon(icon, size: 18, color: color),
        const SizedBox(width: 6),
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(value, style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13, color: color)),
            Text(label, style: TextStyle(fontSize: 11, color: Colors.grey.shade600)),
          ],
        ),
      ],
    );
  }

  Widget _buildFilterBar(BuildContext context, ContinuityPlanLoadedState state) {
    final allCards = state.plan.stages.expand((s) => s.cards).toList();
    final categories = allCards.map((c) => c.categoryName).toSet().toList();

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          children: [
            FilterChip(
              label: const Text('Tất cả'),
              selected: state.selectedCategory == null,
              onSelected: (_) {
                context.read<ContinuityPlanBloc>().add(
                      const FilterContinuityPlanEvent(category: null),
                    );
              },
            ),
            const SizedBox(width: 6),
            ...categories.map((cat) {
              final isSelected = state.selectedCategory == cat;
              return Padding(
                padding: const EdgeInsets.only(right: 6),
                child: FilterChip(
                  label: Text(cat),
                  selected: isSelected,
                  onSelected: (selected) {
                    context.read<ContinuityPlanBloc>().add(
                          FilterContinuityPlanEvent(category: selected ? cat : null),
                        );
                  },
                ),
              );
            }),
            FilterChip(
              avatar: const Icon(Icons.warning_amber_rounded, size: 16, color: Colors.amber),
              label: const Text('Chỉ xem thẻ thiếu sót'),
              selected: state.onlyGaps,
              onSelected: (val) {
                context.read<ContinuityPlanBloc>().add(
                      FilterContinuityPlanEvent(
                        category: state.selectedCategory,
                        onlyGaps: val,
                      ),
                    );
              },
            ),
          ],
        ),
      ),
    );
  }
}
