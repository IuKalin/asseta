import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/continuity_category_entity.dart';
import '../../domain/entities/continuity_item_entity.dart';
import '../bloc/continuity_map_bloc.dart';
import '../bloc/continuity_map_event.dart';
import '../bloc/continuity_map_state.dart';
import '../widgets/category_tile.dart';
import '../widgets/gap_alert_card.dart';
import '../widgets/item_form_bottom_sheet.dart';
import 'assessment_page.dart';

class ContinuityMapPage extends StatefulWidget {
  const ContinuityMapPage({super.key});

  @override
  State<ContinuityMapPage> createState() => _ContinuityMapPageState();
}

class _ContinuityMapPageState extends State<ContinuityMapPage> {
  @override
  void initState() {
    super.initState();
    context.read<ContinuityMapBloc>().add(const LoadContinuityMapEvent());
  }

  void _showUnlockDialog(BuildContext context) {
    final controller = TextEditingController();
    showDialog(
      context: context,
      builder: (dialogCtx) => AlertDialog(
        backgroundColor: const Color(0xFF0F172A),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
        title: const Row(
          children: [
            Icon(Icons.key, color: Colors.amber, size: 22),
            SizedBox(width: 8),
            Text('Mở Khóa Master Key', style: TextStyle(color: Colors.white, fontSize: 16)),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Nhập Passphrase cá nhân để giải mã các ghi chú bí mật ngay trên thiết bị:',
              style: TextStyle(color: Colors.white70, fontSize: 12),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: controller,
              obscureText: true,
              style: const TextStyle(color: Colors.white, fontSize: 13),
              decoration: const InputDecoration(
                hintText: 'Passphrase của bạn...',
                hintStyle: TextStyle(color: Colors.white30, fontSize: 12),
                border: OutlineInputBorder(),
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogCtx).pop(),
            child: const Text('Hủy', style: TextStyle(color: Colors.white60)),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.teal),
            onPressed: () {
              final pass = controller.text.trim();
              if (pass.isNotEmpty) {
                context.read<ContinuityMapBloc>().add(UnlockMasterKeyEvent(pass));
                Navigator.of(dialogCtx).pop();
              }
            },
            child: const Text('Mở Khóa', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  void _openItemBottomSheet(BuildContext context, {ContinuityCategoryEntity? category, ContinuityItemEntity? item}) {
    final state = context.read<ContinuityMapBloc>().state;
    if (state is! ContinuityMapLoaded) return;

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => ItemFormBottomSheet(
        categories: state.map.categories,
        initialCategory: category,
        itemToEdit: item,
        isKeyUnlocked: state.isKeyUnlocked,
        onSubmitCreate: (catId, name, priority, hint, notes) {
          context.read<ContinuityMapBloc>().add(CreateContinuityItemEvent(
                categoryId: catId,
                name: name,
                priority: priority,
                documentLocationHint: hint,
                plainNotes: notes,
              ));
        },
        onSubmitUpdate: (id, name, priority, hint, notes, rowVersion) {
          context.read<ContinuityMapBloc>().add(UpdateContinuityItemEvent(
                id: id,
                name: name,
                priority: priority,
                documentLocationHint: hint,
                plainNotes: notes,
                rowVersion: rowVersion,
              ));
        },
        onUnlockKey: (pass) {
          context.read<ContinuityMapBloc>().add(UnlockMasterKeyEvent(pass));
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF090D16),
      appBar: AppBar(
        backgroundColor: const Color(0xFF0F172A),
        elevation: 0,
        title: const Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Continuity Map', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.white)),
            Text('Zero-Knowledge Succession Vault', style: TextStyle(fontSize: 10, color: Colors.tealAccent)),
          ],
        ),
        actions: [
          BlocBuilder<ContinuityMapBloc, ContinuityMapState>(
            builder: (context, state) {
              final isUnlocked = state is ContinuityMapLoaded && state.isKeyUnlocked;
              return IconButton(
                icon: Icon(
                  isUnlocked ? Icons.lock_open : Icons.lock_outline,
                  color: isUnlocked ? Colors.tealAccent : Colors.amber,
                ),
                tooltip: isUnlocked ? 'Master Key đã mở' : 'Mở khóa ghi chú',
                onPressed: () {
                  if (isUnlocked) {
                    context.read<ContinuityMapBloc>().add(const LockMasterKeyEvent());
                  } else {
                    _showUnlockDialog(context);
                  }
                },
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.assignment_outlined, color: Colors.white70),
            tooltip: 'Khảo sát tiếp quản nhanh',
            onPressed: () {
              Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => const AssessmentPage()),
              );
            },
          ),
        ],
      ),
      body: BlocConsumer<ContinuityMapBloc, ContinuityMapState>(
        listener: (context, state) {
          if (state is ContinuityMapLoaded && state.notificationMessage != null) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.notificationMessage!),
                backgroundColor: const Color(0xFF1E293B),
                duration: const Duration(seconds: 2),
              ),
            );
          }
        },
        builder: (context, state) {
          if (state is ContinuityMapLoading) {
            return const Center(
              child: CircularProgressIndicator(color: Colors.tealAccent),
            );
          } else if (state is ContinuityMapError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error_outline, size: 48, color: Colors.redAccent),
                  const SizedBox(height: 12),
                  Text(state.message, style: const TextStyle(color: Colors.white70)),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () => context.read<ContinuityMapBloc>().add(const LoadContinuityMapEvent()),
                    child: const Text('Thử lại'),
                  ),
                ],
              ),
            );
          } else if (state is ContinuityMapLoaded) {
            final map = state.map;

            return RefreshIndicator(
              color: Colors.tealAccent,
              backgroundColor: const Color(0xFF1E293B),
              onRefresh: () async {
                context.read<ContinuityMapBloc>().add(const RefreshContinuityMapEvent());
              },
              child: ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.only(bottom: 80),
                children: [
                  // Overall Readiness Score Header
                  _buildReadinessHeader(context, map.overallReadinessScore, map.totalItems, map.totalGaps),

                  // Continuity Gap Alert Banner
                  GapAlertCard(
                    gaps: map.gaps,
                    onTapGap: (gap) {
                      for (final cat in map.categories) {
                        final found = cat.items.where((i) => i.id == gap.itemId);
                        if (found.isNotEmpty) {
                          _openItemBottomSheet(context, category: cat, item: found.first);
                          break;
                        }
                      }
                    },
                  ),

                  const Padding(
                    padding: EdgeInsets.only(left: 18, top: 12, bottom: 4),
                    child: Text(
                      'CÁC NHÓM DANH MỤC TIẾP QUẢN',
                      style: TextStyle(
                        fontSize: 11,
                        fontWeight: FontWeight.bold,
                        letterSpacing: 1.0,
                        color: Colors.white60,
                      ),
                    ),
                  ),

                  // 6 Categories Tiles
                  ...map.categories.map((category) {
                    return CategoryTile(
                      category: category,
                      isKeyUnlocked: state.isKeyUnlocked,
                      onAddItem: (cat) => _openItemBottomSheet(context, category: cat),
                      onEditItem: (item) => _openItemBottomSheet(context, item: item),
                      onDeleteItem: (id) {
                        context.read<ContinuityMapBloc>().add(DeleteContinuityItemEvent(id));
                      },
                      onPromptUnlock: () => _showUnlockDialog(context),
                    );
                  }),
                ],
              ),
            );
          }
          return const SizedBox.shrink();
        },
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: Colors.teal,
        icon: const Icon(Icons.add, color: Colors.white),
        label: const Text('Thêm Hạng Mục', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: () => _openItemBottomSheet(context),
      ),
    );
  }

  Widget _buildReadinessHeader(BuildContext context, int score, int totalItems, int totalGaps) {
    Color scoreColor;
    if (score >= 80) {
      scoreColor = Colors.tealAccent;
    } else if (score >= 50) {
      scoreColor = Colors.amber;
    } else {
      scoreColor = Colors.redAccent;
    }

    return Container(
      margin: const EdgeInsets.all(16),
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: const Color(0xFF0F172A).withOpacity(0.9),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: const Color(0xFF1E293B)),
      ),
      child: Row(
        children: [
          // Circular score indicator
          Container(
            width: 72,
            height: 72,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(color: scoreColor, width: 4),
              color: scoreColor.withOpacity(0.1),
            ),
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    '$score%',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: scoreColor,
                    ),
                  ),
                  const Text('READINESS', style: TextStyle(fontSize: 7, color: Colors.white60)),
                ],
              ),
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Chỉ Số Sẵn Sàng Tiếp Quản',
                  style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: Colors.white),
                ),
                const SizedBox(height: 4),
                Text(
                  'Tổng cộng $totalItems hạng mục • ${totalGaps > 0 ? "$totalGaps khoảng trống" : "Toàn vẹn"}',
                  style: const TextStyle(fontSize: 11, color: Colors.white70),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
