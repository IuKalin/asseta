import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/action_card_entity.dart';
import '../../domain/entities/action_card_enums.dart';
import '../bloc/action_card_bloc.dart';
import '../bloc/action_card_event.dart';
import '../bloc/action_card_state.dart';
import '../widgets/urgency_stage_badge.dart';
import 'action_card_detail_page.dart';
import 'action_card_form_page.dart';

class ActionCardsPage extends StatefulWidget {
  const ActionCardsPage({Key? key}) : super(key: key);

  @override
  State<ActionCardsPage> createState() => _ActionCardsPageState();
}

class _ActionCardsPageState extends State<ActionCardsPage> {
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    context.read<ActionCardBloc>().add(const LoadActionCardsEvent());
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Widget _buildUrgencyTab(String title, UrgencyStage? stage, UrgencyStage? currentSelected) {
    final isSelected = currentSelected == stage;
    return GestureDetector(
      onTap: () {
        context.read<ActionCardBloc>().add(FilterUrgencyStageEvent(stage));
      },
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
        margin: const EdgeInsets.only(right: 8),
        decoration: BoxDecoration(
          color: isSelected ? const Color(0xFF0284C7) : const Color(0xFF0F172A),
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: isSelected ? const Color(0xFF38BDF8) : const Color(0xFF1E293B),
          ),
        ),
        child: Text(
          title,
          style: TextStyle(
            color: isSelected ? Colors.white : const Color(0xFF94A3B8),
            fontSize: 12,
            fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
          ),
        ),
      ),
    );
  }

  Widget _buildCardItem(BuildContext context, ActionCardEntity card) {
    final completedSteps = card.steps.where((s) => s.isCompleted).length;
    final totalSteps = card.steps.length;
    final progress = totalSteps > 0 ? (completedSteps / totalSteps) : 0.0;

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      decoration: BoxDecoration(
        color: const Color(0xFF0F172A),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0xFF1E293B)),
      ),
      child: Material(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(16),
        child: InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: () {
            Navigator.push(
              context,
              MaterialPageRoute(
                builder: (_) => BlocProvider.value(
                  value: context.read<ActionCardBloc>(),
                  child: ActionCardDetailPage(cardId: card.id),
                ),
              ),
            );
          },
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    UrgencyStageBadge(stage: card.urgency),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: card.priority == 'CRITICAL'
                            ? const Color(0x33EF4444)
                            : const Color(0x33F59E0B),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        card.priority,
                        style: TextStyle(
                          color: card.priority == 'CRITICAL'
                              ? const Color(0xFFF87171)
                              : const Color(0xFFFBBF24),
                          fontSize: 10,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                Text(
                  card.title,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                if (card.summary != null && card.summary!.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    card.summary!,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: Color(0xFF94A3B8),
                      fontSize: 12,
                    ),
                  ),
                ],
                const SizedBox(height: 12),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Tiến độ: $completedSteps/$totalSteps bước',
                      style: const TextStyle(color: Color(0xFF64748B), fontSize: 11),
                    ),
                    Text(
                      '${(progress * 100).toInt()}%',
                      style: const TextStyle(color: Color(0xFF38BDF8), fontSize: 11, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
                const SizedBox(height: 6),
                ClipRRect(
                  borderRadius: BorderRadius.circular(4),
                  child: LinearProgressIndicator(
                    value: progress,
                    minHeight: 5,
                    backgroundColor: const Color(0xFF1E293B),
                    valueColor: AlwaysStoppedAnimation<Color>(
                      progress == 1.0 ? const Color(0xFF10B981) : const Color(0xFF0284C7),
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    if (card.hasConfidentialInstructions) ...[
                      const Icon(Icons.lock, size: 14, color: Color(0xFFA855F7)),
                      const SizedBox(width: 4),
                      const Text(
                        'Mật mã AES-256',
                        style: TextStyle(color: Color(0xFFA855F7), fontSize: 11),
                      ),
                      const SizedBox(width: 12),
                    ],
                    if (card.contacts.isNotEmpty) ...[
                      const Icon(Icons.contact_phone, size: 14, color: Color(0xFF64748B)),
                      const SizedBox(width: 4),
                      Text(
                        '${card.contacts.length} liên hệ',
                        style: const TextStyle(color: Color(0xFF64748B), fontSize: 11),
                      ),
                    ],
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF020617), // Slate 950
      appBar: AppBar(
        backgroundColor: const Color(0xFF020617),
        elevation: 0,
        title: const Text(
          'Action Cards',
          style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh, color: Colors.white70),
            onPressed: () {
              context.read<ActionCardBloc>().add(const LoadActionCardsEvent());
            },
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: const Color(0xFF0284C7),
        icon: const Icon(Icons.add, color: Colors.white),
        label: const Text('Tạo Thẻ Mới', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: () {
          Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => BlocProvider.value(
                value: context.read<ActionCardBloc>(),
                child: const ActionCardFormPage(),
              ),
            ),
          );
        },
      ),
      body: BlocBuilder<ActionCardBloc, ActionCardState>(
        builder: (context, state) {
          if (state is ActionCardLoading) {
            return const Center(child: CircularProgressIndicator(color: Color(0xFF38BDF8)));
          }

          if (state is ActionCardError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline, size: 48, color: Colors.redAccent),
                    const SizedBox(height: 12),
                    Text(
                      state.message,
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: Colors.white70),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF0284C7)),
                      onPressed: () {
                        context.read<ActionCardBloc>().add(const LoadActionCardsEvent());
                      },
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is ActionCardLoaded) {
            final cards = state.filteredCards;

            return RefreshIndicator(
              color: const Color(0xFF38BDF8),
              backgroundColor: const Color(0xFF0F172A),
              onRefresh: () async {
                context.read<ActionCardBloc>().add(const LoadActionCardsEvent());
              },
              child: CustomScrollView(
                slivers: [
                  // Search Bar
                  SliverToBoxAdapter(
                    child: Padding(
                      padding: const EdgeInsets.fromLTRB(16, 8, 16, 12),
                      child: TextField(
                        controller: _searchController,
                        style: const TextStyle(color: Colors.white),
                        decoration: InputDecoration(
                          hintText: 'Tìm kiếm thẻ, bối cảnh...',
                          hintStyle: const TextStyle(color: Color(0xFF64748B)),
                          prefixIcon: const Icon(Icons.search, color: Color(0xFF64748B)),
                          filled: true,
                          fillColor: const Color(0xFF0F172A),
                          contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(12),
                            borderSide: const BorderSide(color: Color(0xFF1E293B)),
                          ),
                          enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(12),
                            borderSide: const BorderSide(color: Color(0xFF1E293B)),
                          ),
                        ),
                        onChanged: (val) {
                          context.read<ActionCardBloc>().add(SearchActionCardsEvent(val));
                        },
                      ),
                    ),
                  ),

                  // Urgency Stage Horizontal Pills
                  SliverToBoxAdapter(
                    child: Container(
                      height: 42,
                      margin: const EdgeInsets.only(bottom: 16),
                      child: ListView(
                        scrollDirection: Axis.horizontal,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        children: [
                          _buildUrgencyTab('Tất cả', null, state.selectedUrgency),
                          _buildUrgencyTab('⚡ Ngay lập tức', UrgencyStage.immediate, state.selectedUrgency),
                          _buildUrgencyTab('⏳ 72 Giờ đầu', UrgencyStage.first72Hours, state.selectedUrgency),
                          _buildUrgencyTab('📅 7 Ngày đầu', UrgencyStage.first7Days, state.selectedUrgency),
                          _buildUrgencyTab('🛡️ Dài hạn', UrgencyStage.longerTerm, state.selectedUrgency),
                        ],
                      ),
                    ),
                  ),

                  // Cards List or Empty
                  if (cards.isEmpty)
                    SliverFillRemaining(
                      hasScrollBody: false,
                      child: Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: const [
                            Icon(Icons.style_outlined, size: 56, color: Color(0xFF334155)),
                            SizedBox(height: 12),
                            Text(
                              'Không có thẻ hành động nào',
                              style: TextStyle(color: Color(0xFF64748B), fontSize: 14),
                            ),
                          ],
                        ),
                      ),
                    )
                  else
                    SliverPadding(
                      padding: const EdgeInsets.symmetric(horizontal: 16),
                      sliver: SliverList(
                        delegate: SliverChildBuilderDelegate(
                          (context, index) => _buildCardItem(context, cards[index]),
                          childCount: cards.length,
                        ),
                      ),
                    ),
                ],
              ),
            );
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }
}
