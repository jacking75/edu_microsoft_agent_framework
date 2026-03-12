# Stage 14: Balance Simulator

> **학습 목표**: 몬테카를로 시뮬레이션과 최적화

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 |
|----------|------|-----------|
| **14A_MonteCarloSimulator** | ✅ 몬테카를로 전투 시뮬레이션 | 확률시뮬레이션, 대량실행 |
| **14B_ParameterOptimizer** | ⏸️ 파라미터 최적화 (확장용) | 그리드서치, 결과시각화 |
| **14C_RLBalancer** | ⏸️ 강화학습 밸런서 (확장용) | 자기플레이, 메타분석 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 14A 실행 (완전 구현됨)
dotnet run --project Stage14_BalanceSimulator/14A_MonteCarloSimulator
```

---

## 🎯 14A_MonteCarloSimulator 학습 내용

### 몬테카를로 시뮬레이션 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  BattleParams   │────▶│   MonteCarlo    │────▶│SimulationResult │
│  (공격/수비)     │     │   Simulator     │     │  (승률/분석)    │
└─────────────────┘     └────────┬────────┘     └────────┬────────┘
                                 │                       │
                                 ▼                       ▼
                         ┌─────────────────┐     ┌─────────────────┐
                         │  10,000 회 반복   │     │   AnalysisAgent │
                         │  (난수시뮬레이션)  │     │   (AI 해석)     │
                         └─────────────────┘     └─────────────────┘
```

### 1. 전투 시뮬레이션 엔진

```csharp
public class BattleSimulationEngine
{
    public BattleResult SimulateBattle(BattleParams attacker, BattleParams defender)
    {
        while (attackerHp > 0 && defenderHp > 0)
        {
            // Crit 계산
            var isCrit = _random.NextDouble() < attacker.CritChance;
            var dmg = attacker.Damage * (isCrit ? attacker.CritMultiplier : 1);
            
            // 데미지 적용
            defenderHp -= (int)(dmg * (1 - defender.DefenseReduction));
        }
        
        return result;
    }
}
```

### 2. 몬테카를로 실행

```csharp
public SimulationResult RunMonteCarlo(
    BattleParams attacker, 
    BattleParams defender, 
    int iterations = 10000)
{
    for (int i = 0; i < iterations; i++)
    {
        var result = SimulateBattle(attacker, defender);
        // 결과 집계
    }
    
    return new SimulationResult
    {
        AttackerWinRate = attackerWins / iterations * 100,
        DefenderWinRate = defenderWins / iterations * 100,
        AverageRounds = totalRounds / iterations
    };
}
```

### 3. AI 분석 에이전트

```csharp
public class MonteCarloAnalysisAgent
{
    public async Task<string> AnalyzeResultAsync(SimulationResult result)
    {
        var prompt = $"""
            시뮬레이션 결과를 분석하세요:
            - 공격자 승률: {result.AttackerWinRate:F2}%
            - 수비자 승률: {result.DefenderWinRate:F2}%
            
            밸런스 평가와 조정 방안을 제안하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

---

## 💡 실행 예시

```
🎲 몬테카를로 전투 시뮬레이터에 오신 것을 환영합니다!

✅ OpenAI API 키가 설정되었습니다.
✅ 시뮬레이션 엔진이 초기화되었습니다.

📊 기본 전투 파라미터:
공격자: HP 1000, Damage 100, Crit 15%
수비자: HP 1200, Damage 80, Crit 10%

🎲 몬테카를로 시뮬레이션 실행 중... (10,000 회)

📊 시뮬레이션 결과:
  공격자 승률: 52.34%
  수비자 승률: 47.66%
  무승부: 0.00%
  평균 라운드: 8.45

🤖 AI 분석 중...

📋 분석 결과:
시뮬레이션결과요약: 공격자가 약간 우세한 밸런스
밸런스평가: 양호한 밸런스 (승률 45-55% 범위 내)
조정방안: 현재 상태 유지 권장
권장파라미터: 수비자 HP 1150-1200 범위 권장
```

---

## 📊 밸런스 평가 기준

| 승률 차이 | 평가 | 조치 |
|-----------|------|------|
| **45-55%** | ✅ 양호 | 유지 |
| **40-60%** | ⚠️ 허용 | 모니터링 |
| **<40% / >60%** | ❌ 불량 | 조정필요 |

---

## ✅ 완료된 Stage

- [x] Stage 01: 텍스트 응답
- [x] Stage 02: 단일 Tool
- [x] Stage 03: 파일 시스템
- [x] Stage 04: 다중 Tool
- [x] Stage 05: 외부 API
- [x] Stage 06: 데이터베이스
- [x] Stage 07: RAG
- [x] Stage 08: Multi-Agent (Sequential)
- [x] Stage 09: Workflow QA (Graph)
- [x] Stage 10: Human-in-the-Loop
- [x] Stage 11: Real-time Analysis
- [x] Stage 12: Multimodal
- [x] Stage 13: PM Assistant
- [x] **Stage 14: Balance Simulator**

총 **42 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[몬테카를로 방법](https://learn.microsoft.com/azure/machine-learning/how-to-monte-carlo-simulation)**
- **[게임 밸런싱](https://learn.microsoft.com/games/game-balance)**
- **[시뮬레이션 최적화](https://learn.microsoft.com/azure/optimization/simulation)**
