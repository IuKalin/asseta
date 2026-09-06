import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/core/crypto/crypto_service.dart';
import 'package:asseta_mobile/features/continuity_map/domain/entities/continuity_map_entity.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_bloc.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/bloc/continuity_map_state.dart';
import 'package:asseta_mobile/features/continuity_map/presentation/pages/continuity_map_page.dart';
import 'features/continuity_map/presentation/continuity_map_page_test.dart';

void main() {
  testWidgets('AssetaApp smoke test renders Continuity Map', (WidgetTester tester) async {
    const testMap = ContinuityMapEntity(
      overallReadinessScore: 80,
      totalItems: 0,
      totalGaps: 0,
      categories: [],
      gaps: [],
    );

    final mockRepo = MockRepoForWidgetTest(testMap);
    final cryptoService = MobileCryptoService();
    final bloc = ContinuityMapBloc(repository: mockRepo, cryptoService: cryptoService);
    bloc.emit(const ContinuityMapLoaded(map: testMap));

    await tester.pumpWidget(
      MaterialApp(
        home: BlocProvider<ContinuityMapBloc>.value(
          value: bloc,
          child: const ContinuityMapPage(),
        ),
      ),
    );
    await tester.pump();

    expect(find.text('Continuity Map'), findsOneWidget);
  });
}
