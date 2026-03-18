# Skill: Calculator

## Description
게임 밸런스 계산을 수행하는 스킬입니다.
DPS, 크리티컬 데미지, 기대 데미지 등 다양한 전투 통계를 계산할 수 있습니다.

## Functions

### calculate_dps
- **설명**: 무기의 초당 데미지 (DPS) 를 계산합니다.
- **파라미터**:
- damage: int (무기의 기본 공격력)
- attacksPerSecond: double (초당 공격 속도)
- **수식**: damage * attacksPerSecond

### calculate_crit_damage
- **설명**: 크리티컬 발생 시 입히는 데미지를 계산합니다.
- **파라미터**:
- baseDamage: double (기본 데미지)
- critMultiplier: double (크리티컬 배율, 예: 2.0 은 2 배)
- **수식**: baseDamage * critMultiplier

### calculate_expected_dps
- **설명**: 크리티컬 확률을 고려한 기대 DPS 를 계산합니다.
- **파라미터**:
- dps: double (기본 DPS)
- critChance: double (크리티컬 확률, 0-1 범위)
- critMultiplier: double (크리티컬 배율)
- **수식**: dps * (1 + (critChance * (critMultiplier - 1)))

## Keywords
- dps
- damage
- attack
- damage per second
- crit
- critical
- 기대값
- 확률
- 배율
- 계산
- 공격력
- 공격속도
- 크리티컬

## Examples
- "공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘"
- "기본 데미지 200 이 크리티컬로 맞으면 몇이야? (배율 2.5)"
- "DPS 150, 크리티컬 확률 30%, 배율 2 배일 때 기대 DPS 는?"
- "공격력 80, 초당 2 회 공격하는 무기의 성능을 분석해줘"
