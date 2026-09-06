import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'core/crypto/crypto_service.dart';
import 'core/network/idempotency_interceptor.dart';
import 'core/theme/app_theme.dart';
import 'features/continuity_map/data/datasources/continuity_local_datasource.dart';
import 'features/continuity_map/data/datasources/continuity_remote_datasource.dart';
import 'features/continuity_map/data/repositories/continuity_repository_impl.dart';
import 'features/continuity_map/presentation/bloc/continuity_map_bloc.dart';
import 'features/continuity_map/presentation/pages/continuity_map_page.dart';
import 'features/action_cards/data/datasources/action_card_local_datasource.dart';
import 'features/action_cards/data/datasources/action_card_remote_datasource.dart';
import 'features/action_cards/data/repositories/action_card_repository_impl.dart';
import 'features/action_cards/presentation/bloc/action_card_bloc.dart';
import 'features/action_cards/presentation/bloc/action_card_event.dart';
import 'features/action_cards/presentation/pages/action_cards_page.dart';
import 'features/safe_activation/data/datasources/safe_activation_remote_datasource.dart';
import 'features/safe_activation/data/repositories/safe_activation_repository_impl.dart';
import 'features/safe_activation/presentation/bloc/safe_activation_bloc.dart';
import 'features/safe_activation/presentation/bloc/safe_activation_event.dart';
import 'features/safe_activation/presentation/pages/safe_activation_page.dart';

void main() {
  runApp(const AssetaApp());
}

class AssetaApp extends StatefulWidget {
  const AssetaApp({super.key});

  @override
  State<AssetaApp> createState() => _AssetaAppState();
}

class _AssetaAppState extends State<AssetaApp> {
  int _currentIndex = 0;

  @override
  Widget build(BuildContext context) {
    final dio = Dio(BaseOptions(
      baseUrl: 'http://localhost:5000/api/v1',
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
    ));
    dio.interceptors.add(IdempotencyInterceptor());

    final continuityRemote = ContinuityRemoteDataSourceImpl(dio: dio);
    final continuityLocal = InMemoryContinuityLocalDataSource();
    final continuityRepo = ContinuityRepositoryImpl(
      remoteDataSource: continuityRemote,
      localDataSource: continuityLocal,
    );

    final actionCardRemote = ActionCardRemoteDataSourceImpl(dio: dio);
    final actionCardLocal = InMemoryActionCardLocalDataSource();
    final actionCardRepo = ActionCardRepositoryImpl(
      remoteDataSource: actionCardRemote,
      localDataSource: actionCardLocal,
    );

    final safeActivationRemote = SafeActivationRemoteDataSourceImpl(dio: dio);
    final safeActivationRepo = SafeActivationRepositoryImpl(remoteDataSource: safeActivationRemote);

    final cryptoService = MobileCryptoService();

    final pages = [
      const ContinuityMapPage(),
      const ActionCardsPage(),
      const SafeActivationPage(),
    ];

    return MultiBlocProvider(
      providers: [
        BlocProvider<ContinuityMapBloc>(
          create: (_) => ContinuityMapBloc(
            repository: continuityRepo,
            cryptoService: cryptoService,
          ),
        ),
        BlocProvider<ActionCardBloc>(
          create: (_) => ActionCardBloc(
            repository: actionCardRepo,
          )..add(const LoadActionCardsEvent()),
        ),
        BlocProvider<SafeActivationBloc>(
          create: (_) => SafeActivationBloc(
            repository: safeActivationRepo,
          )..add(const FetchSafeActivationStatusEvent()),
        ),
      ],
      child: MaterialApp(
        title: 'Asseta Monorepo',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.darkTheme,
        home: Scaffold(
          body: IndexedStack(
            index: _currentIndex,
            children: pages,
          ),
          bottomNavigationBar: BottomNavigationBar(
            currentIndex: _currentIndex,
            backgroundColor: const Color(0xFF0F172A),
            selectedItemColor: const Color(0xFF38BDF8),
            unselectedItemColor: const Color(0xFF64748B),
            onTap: (index) {
              setState(() {
                _currentIndex = index;
              });
            },
            items: const [
              BottomNavigationBarItem(
                icon: Icon(Icons.dashboard_outlined),
                activeIcon: Icon(Icons.dashboard),
                label: 'Bản đồ liên tục',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.emergency_outlined),
                activeIcon: Icon(Icons.emergency),
                label: 'Thẻ hành động',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.security_outlined),
                activeIcon: Icon(Icons.security),
                label: 'Kích hoạt',
              ),
            ],
          ),
        ),
      ),
    );
  }
}
