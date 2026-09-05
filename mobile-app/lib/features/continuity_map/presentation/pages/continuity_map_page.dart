import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../bloc/continuity_map_bloc.dart';
import '../../data/repositories/continuity_map_repository_impl.dart';

class ContinuityMapPage extends StatelessWidget {
  const ContinuityMapPage({super.key});

  @override
  Widget build(BuildContext context) {
    const emeraldColor = Color(0xFF10B981);

    return BlocProvider(
      create: (context) => ContinuityMapBloc(
        repository: ContinuityMapRepositoryImpl(),
      )..add(LoadContinuityMapEvent()),
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Asseta Continuity Map'),
          actions: [
            Container(
              margin: const EdgeInsets.only(right: 16),
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: emeraldColor.withValues(alpha: 0.2),
                borderRadius: BorderRadius.circular(6),
                border: Border.all(color: emeraldColor.withValues(alpha: 0.4)),
              ),
              child: const Text(
                'v1.0.0 APPROVED',
                style: TextStyle(color: Colors.greenAccent, fontSize: 11, fontWeight: FontWeight.bold),
              ),
            ),
          ],
        ),
        body: BlocBuilder<ContinuityMapBloc, ContinuityMapState>(
          builder: (context, state) {
            if (state is ContinuityMapLoading) {
              return const Center(child: CircularProgressIndicator());
            } else if (state is ContinuityMapLoaded) {
              return ListView.builder(
                padding: const EdgeInsets.all(16),
                itemCount: state.assets.length,
                itemBuilder: (context, index) {
                  final asset = state.assets[index];
                  return Card(
                    margin: const EdgeInsets.only(bottom: 12),
                    color: const Color(0xFF1E293B),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                      side: const BorderSide(color: Color(0xFF334155)),
                    ),
                    child: ListTile(
                      title: Text(
                        asset.name,
                        style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
                      ),
                      subtitle: Text(
                        asset.description,
                        style: const TextStyle(color: Colors.grey),
                      ),
                      trailing: Text(
                        '\$${asset.estimatedValue.toStringAsFixed(0)}',
                        style: const TextStyle(
                          color: Color(0xFF10B981),
                          fontWeight: FontWeight.bold,
                          fontSize: 16,
                        ),
                      ),
                    ),
                  );
                },
              );
            } else if (state is ContinuityMapError) {
              return Center(child: Text('Lỗi: ${state.message}'));
            }
            return const SizedBox.shrink();
          },
        ),
      ),
    );
  }
}
