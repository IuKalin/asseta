import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../bloc/trusted_people_bloc.dart';

class PairingCodeClaimPage extends StatefulWidget {
  const PairingCodeClaimPage({super.key});

  @override
  State<PairingCodeClaimPage> createState() => _PairingCodeClaimPageState();
}

class _PairingCodeClaimPageState extends State<PairingCodeClaimPage> {
  final _codeController = TextEditingController();

  @override
  void dispose() {
    _codeController.dispose();
    super.dispose();
  }

  void _submitCode() {
    final code = _codeController.text.trim();
    if (code.length == 6) {
      context.read<TrustedPeopleBloc>().add(ClaimPairingCodeEvent(code));
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Ghép Đôi Người Ủy Thác'),
      ),
      body: BlocConsumer<TrustedPeopleBloc, TrustedPeopleState>(
        listener: (context, state) {
          if (state is TrustedPeopleErrorState) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.redAccent),
            );
          }
          if (state is PairingClaimSuccessState) {
            showDialog(
              context: context,
              builder: (ctx) => AlertDialog(
                icon: const Icon(Icons.check_circle, color: Colors.green, size: 48),
                title: const Text('Ghép Đôi Thành Công!'),
                content: Text(
                  'Bạn đã được liên kết với vai trò:\n\n'
                  '${state.result.assignedRole}\n\n'
                  'Ủy thác bởi: ${state.result.ownerDisplayName}',
                  textAlign: TextAlign.center,
                ),
                actions: [
                  ElevatedButton(
                    onPressed: () {
                      Navigator.pop(ctx);
                      Navigator.pop(context);
                    },
                    child: const Text('Đóng'),
                  ),
                ],
              ),
            );
          }
        },
        builder: (context, state) {
          final isLoading = state is TrustedPeopleLoadingState;

          return Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                const SizedBox(height: 32),
                Icon(Icons.shield_outlined, size: 72, color: theme.colorScheme.primary),
                const SizedBox(height: 24),
                Text(
                  'Nhập Mã Ghép Đôi',
                  style: theme.textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                Text(
                  'Nhập mã 6 ký tự được cung cấp bởi chủ tài sản để hoàn tất liên kết tài khoản tiếp quản.',
                  textAlign: TextAlign.center,
                  style: theme.textTheme.bodyMedium?.copyWith(color: Colors.grey),
                ),
                const SizedBox(height: 32),
                TextField(
                  controller: _codeController,
                  textCapitalization: TextCapitalization.characters,
                  textAlign: TextAlign.center,
                  maxLength: 6,
                  style: const TextStyle(fontSize: 32, letterSpacing: 8, fontWeight: FontWeight.bold),
                  decoration: InputDecoration(
                    hintText: 'AS7K9P',
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(16)),
                    counterText: '',
                  ),
                  onChanged: (val) {
                    if (val.length == 6) {
                      _submitCode();
                    }
                  },
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  height: 48,
                  child: ElevatedButton(
                    onPressed: isLoading ? null : _submitCode,
                    child: isLoading
                        ? const CircularProgressIndicator(strokeWidth: 2)
                        : const Text('Xác Nhận Ghép Đôi'),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
