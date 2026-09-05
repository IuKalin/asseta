import 'package:flutter/material.dart';
import 'core/theme/app_theme.dart';
import 'features/continuity_map/presentation/pages/continuity_map_page.dart';

void main() {
  runApp(const AssetaApp());
}

class AssetaApp extends StatelessWidget {
  const AssetaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Asseta Monorepo Mobile',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.darkTheme,
      home: const ContinuityMapPage(),
    );
  }
}
