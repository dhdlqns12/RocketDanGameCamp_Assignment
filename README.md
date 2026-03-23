# ⭐ Star Defense: 유즈맵 TD 모작
 
스타크래프트 유즈맵 디펜스를 모티브로 한 2D 타워 디펜스 모바일 게임
 
> **Unity 2022.3.62f3** | **개발 기간**: 2026.03.19 ~ 2026.03.23 (5일)
 
---
 
## 📋 프로젝트 개요
 
게임캠프 클라이언트 개발자 과제전형으로 제작한 TD(Tower Defense) 게임입니다.
원작 "스타 디펜스"의 핵심 시스템을 Unity로 재현하였으며, 데이터 드리븐 설계와 확장 가능한 아키텍처에 중점을 두었습니다.
 
---
 
## 🎮 주요 기능
 
### 영웅 시스템
- **소환**: 확률 기반 랜덤 소환 (10,000분율), 범용 프리팹 1개로 데이터 드리븐 생성
- **승급**: 같은 영웅 2기 → 다음 등급 랜덤 유닛으로 승급 (Common → Rare → Epic → Unique)
- **초월**: Unique 등급 → Legend 등급, 진화트리에 따라 1~3개 선택지 제공
- **전략 패턴**: splashRadius에 따라 단일 공격 / 범위 공격 자동 결정
 
### 적 시스템
- **FSM 기반 AI**: Spawn → Move → Die / ReachEnd 상태 머신, 상태 객체 캐싱으로 GC 최소화
- **피격 이펙트**: 커스텀 SpriteWhiteFlash 셰이더 + DOTween 연동
- **오브젝트 풀링**: EnemyPool로 적 재사용
 
### 웨이브 시스템
- phase별 적 스폰, 웨이브 간 보간 카운트 지원
- 자동 진행 (첫 웨이브 대기 → 스폰 → 대기 → 다음 웨이브)
 
### 강화 시스템
- 등급별 공격력 강화 (Common+Rare / Epic / Unique+Legend), 뽑기 확률 강화
- 비용 레벨당 10% 증가 (반올림), 기본 데미지 기준 보너스 적용
- 신규 소환 영웅에도 기존 강화 보너스 자동 적용
 
### 현상금 시스템
- EnemyData의 `isBounty` 플래그로 자동 수집, 데이터 추가만으로 확장
- 개별 슬롯 (스프라이트 + 이름 + HP + 보상), GridLayoutGroup 동적 생성
- 전체 쿨타임 (1초 단위 이벤트 Tick)
 
### 탐사정 시스템
- 넥서스 ↔ 좌우 광산 왕복, 1회 왕복 = 미네랄 +1
- 홀수 프로브: 오른쪽 / 짝수 프로브: 왼쪽
- 구매 비용 20% 증가, 최대 20개 (확장 가능)
 
### 맵 시스템
- JSON 기반 맵 데이터 (7종 타일 타입)
- 타일 수리 (FixBlock → Block), 버프 타일 (공속 30% 증가)
- 챕터별 테마 프리팹 자동 적용
 
---
 
## 🏗️ 아키텍처
 
### 설계 원칙
- **데이터 드리븐**: 영웅/적/스테이지/웨이브 전부 JSON으로 관리, 코드 변경 없이 콘텐츠 확장
- **이벤트 기반 통신**: 골드/미네랄/웨이브/쿨타임 등 `event Action`으로 느슨한 결합
- **Namespace별 역할 분리**: Core, Data, Hero, Enemy, Managers, Map, UI
 
### UI 아키텍처 (UIBase + UIManager)
 
```
UIManager (ManagerRoot 하위, 전역 관리)
├── sceneCanvas     — Scene UI (항상 표시: BottomHudUI, TopHudUI)
├── popupCanvas     — Popup UI (하나만 열림: 강화/현상금/탐사정/스탯 패널)
└── tilePopupCanvas — TilePopup UI (하나만 열림: 소환/승급/수리/초월 버튼)
```
 
- **UIBase**: 추상 클래스, `SetupUI()` / `SubscribeEvents()` / `UnsubscribeEvents()` 통일
- **UIType별 배타적 표시**: 같은 타입은 하나만 열림
- **의존성 주입**: `SetDependencies()`로 매니저 참조 전달, Inspector 의존 최소화

## 📁 데이터 구조
 
### HeroData.json
```json
{
  "heroId": 2301,
  "heroName": "UniqueSanta",
  "rarity": "Unique",
  "tribe": "Human",
  "attackDamage": 80,
  "attackSpeed": 1.2,
  "attackRange": 3.5,
  "splashRadius": 0,
  "heroSprite": 2301,
  "projectileSprite": 3301,
  "transcendOptions": [2401, 2402]
}
```
 
### EnemyData.json
```json
{
  "enemyId": 9001,
  "enemyName": "GoldBounty",
  "hp": 300,
  "moveSpeed": 1.8,
  "damage": 0,
  "goldReward": 50,
  "mineralReward": 0,
  "isAir": false,
  "isBounty": true
}
```
 
---
 
## 🛠️ 기술 스택
 
- **엔진**: Unity 2022.3.62f3
- **언어**: C#
- **JSON 파싱**: Newtonsoft.Json
- **애니메이션**: DOTween
- **셰이더**: Custom SpriteWhiteFlash (피격 이펙트)
- **UI**: TextMeshPro, Unity UI (Canvas)
 
---
 
## 📸 시연영상
[![플레이 영상](https://img.youtube.com/vi/Zua7teOQ7Wo/0.jpg)](https://www.youtube.com/watch?v=Zua7teOQ7Wo)
 
