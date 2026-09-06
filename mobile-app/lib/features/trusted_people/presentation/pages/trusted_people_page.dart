import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../bloc/trusted_people_bloc.dart';
import '../widgets/trusted_person_card.dart';

class TrustedPeoplePage extends StatefulWidget {
  const TrustedPeoplePage({super.key});

  @override
  State<TrustedPeoplePage> createState() => _TrustedPeoplePageState();
}

class _TrustedPeoplePageState extends State<TrustedPeoplePage> {
  @override
  void initState() {
    super.initState();
    context.read<TrustedPeopleBloc>().add(const LoadTrustedPeopleEvent());
  }

  void _showAddPersonDialog() {
    final nameCtrl = TextEditingController();
    final emailCtrl = TextEditingController();
    final phoneCtrl = TextEditingController();
    final relCtrl = TextEditingController();
    final roleCtrl = TextEditingController();
    int selectedTrustLevel = 2;

    showDialog(
      context: context,
      builder: (dialogCtx) {
        return StatefulBuilder(
          builder: (context, setDialogState) {
            return AlertDialog(
              title: const Text('Thêm Người Ủy Thác'),
              content: SingleChildScrollView(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    TextField(controller: nameCtrl, decoration: const InputDecoration(labelText: 'Họ và tên *')),
                    TextField(controller: emailCtrl, decoration: const InputDecoration(labelText: 'Email *')),
                    TextField(controller: phoneCtrl, decoration: const InputDecoration(labelText: 'Số điện thoại *')),
                    TextField(controller: relCtrl, decoration: const InputDecoration(labelText: 'Mối quan hệ *')),
                    TextField(controller: roleCtrl, decoration: const InputDecoration(labelText: 'Mô tả vai trò')),
                    const SizedBox(height: 12),
                    DropdownButtonFormField<int>(
                      value: selectedTrustLevel,
                      decoration: const InputDecoration(labelText: 'Cấp bậc tin cậy'),
                      items: const [
                        DropdownMenuItem(value: 1, child: Text('Level 1: Notice Only')),
                        DropdownMenuItem(value: 2, child: Text('Level 2: Scoped Delegate')),
                        DropdownMenuItem(value: 3, child: Text('Level 3: Primary Delegate')),
                      ],
                      onChanged: (val) {
                        if (val != null) setDialogState(() => selectedTrustLevel = val);
                      },
                    ),
                  ],
                ),
              ),
              actions: [
                TextButton(onPressed: () => Navigator.pop(dialogCtx), child: const Text('Hủy')),
                ElevatedButton(
                  onPressed: () {
                    if (nameCtrl.text.isNotEmpty && emailCtrl.text.isNotEmpty && phoneCtrl.text.isNotEmpty) {
                      context.read<TrustedPeopleBloc>().add(
                            CreateTrustedPersonEvent(
                              fullName: nameCtrl.text.trim(),
                              email: emailCtrl.text.trim(),
                              phoneNumber: phoneCtrl.text.trim(),
                              relationship: relCtrl.text.trim(),
                              trustLevel: selectedTrustLevel,
                              roleDescription: roleCtrl.text.trim(),
                            ),
                          );
                      Navigator.pop(dialogCtx);
                    }
                  },
                  child: const Text('Thêm'),
                ),
              ],
            );
          },
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Người Được Ủy Thác'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => context.read<TrustedPeopleBloc>().add(const LoadTrustedPeopleEvent()),
          ),
        ],
      ),
      body: BlocConsumer<TrustedPeopleBloc, TrustedPeopleState>(
        listener: (context, state) {
          if (state is TrustedPeopleErrorState) {
            ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(state.message)));
          }
          if (state is TrustedPeopleLoadedState && state.newlyCreatedPairingCode != null) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('Mã ghép đôi tạo mới: ${state.newlyCreatedPairingCode} (Hiệu lực 48h)'),
                backgroundColor: Colors.green,
                duration: const Duration(seconds: 5),
              ),
            );
          }
        },
        builder: (context, state) {
          if (state is TrustedPeopleLoadingState) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is TrustedPeopleLoadedState) {
            if (state.people.isEmpty) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.people_outline, size: 64, color: Colors.grey),
                    const SizedBox(height: 16),
                    const Text('Chưa có người ủy thác nào', style: TextStyle(fontSize: 16, color: Colors.grey)),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      icon: const Icon(Icons.add),
                      label: const Text('Thêm Người Đầu Tiên'),
                      onPressed: _showAddPersonDialog,
                    ),
                  ],
                ),
              );
            }

            return ListView.builder(
              itemCount: state.people.length,
              itemBuilder: (context, index) {
                final person = state.people[index];
                return TrustedPersonCard(
                  person: person,
                  onRevoke: () => context.read<TrustedPeopleBloc>().add(RevokeTrustedPersonEvent(person.id)),
                  onRegenerateCode: () => context.read<TrustedPeopleBloc>().add(RegeneratePairingCodeEvent(person.id)),
                );
              },
            );
          }

          return const SizedBox.shrink();
        },
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _showAddPersonDialog,
        icon: const Icon(Icons.person_add),
        label: const Text('Thêm'),
      ),
    );
  }
}
