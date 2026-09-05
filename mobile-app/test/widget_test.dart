import 'package:flutter_test/flutter_test.dart';
import 'package:asseta_mobile/main.dart';

void main() {
  testWidgets('AssetaApp basic widget smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(const AssetaApp());
    expect(find.text('Asseta Continuity Map'), findsOneWidget);
  });
}
