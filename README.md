# roller

16세기 동아시아 다크 판타지 용병 RPG (모바일, Android 우선).

- 장르: 2D 그리드 RTwP(실시간 일시정지) 파티 전술 RPG
- 참조: Battle Brothers + Pillars of Eternity + FF12 (감빗 시스템)
- 플랫폼: Android (가로), iOS 후속
- 엔진: Unity 6 LTS, 2D URP
- 세션 길이: 5–10분 (의뢰 1건)
- 수익 모델: F2P + AdMob 광고 + 광고 제거 IAP

## 현재 상태

- [x] 설계 문서 완성 (`docs/design/`)
- [x] 마일스톤 1 코드 스캐폴드 작성 (`Assets/Scripts/`)
- [ ] Unity 프로젝트 생성 (사용자 GUI 작업, `SETUP.md` 참조)
- [ ] 마일스톤 1 검증 (8×6 그리드, 캐릭터 1명 탭 이동, Android 빌드)
- [ ] 마일스톤 2 ~ 12

## 문서

| 위치 | 내용 |
|------|------|
| `docs/design/README.md` | 디자인 문서 인덱스 |
| `docs/design/mercenaries.md` | 용병 9명 |
| `docs/design/monsters.md` | 몬스터 8종 |
| `docs/design/skills.md` | 스킬 시스템 + 캐릭터 키트 |
| `docs/design/contracts.md` | 의뢰 25개 |
| `docs/design/imjin-war.md` | 임진왜란 분기 |
| `docs/design/milestone-1-implementation.md` | M1 상세 구현 가이드 |
| `SETUP.md` | Unity 프로젝트 셋업 단계별 가이드 |

## 빠른 시작

1. `SETUP.md` 따라 Unity 설치 + 프로젝트 열기
2. Player Settings 가로 모드 락
3. `Assets/Scenes/M1Demo.unity` 씬 만들고 GameObject 배치 (SETUP 안내)
4. Play 버튼 → 그리드에 캐릭터 1명. 탭하면 이동
5. Android 빌드 → APK 설치 → 실기에서 동일하게 동작

## 코드 구조 (M1 기준)

```
Assets/
  Scripts/
    Core/           Coord, Direction
    Grid/           Tile/지형 시스템 + 입력 + 렌더링
    Pathfinding/    A* + 단순 우선순위 큐
    Characters/     캐릭터 엔티티 + 이동 코루틴
    Camera/         모바일 핀치/팬 카메라
    Demo/           M1DemoController (씬 진입점)
```

## 라이선스

(미정 — 개인 프로젝트, 출시 시 결정)
