# Stage 06: Database - 데이터베이스 연동 학습

## 📋 개요

Stage 06 에서는 AI Agent 가 **데이터베이스와 안전하게 연동하는 방법**을 학습합니다. SQLite 를 사용하여 CRUD 작업, 파라미터화된 쿼리로 SQL 인젝션 방지, 자연어를 SQL 로 변환하는 패턴을 이해합니다.

## 🎯 학습 목표

1. **SQLite 연동**: `Microsoft.Data.Sqlite` 를 사용한 DB 접근
2. **파라미터화된 쿼리**: SQL 인젝션 공격 방지
3. **안전한 쿼리 검증**: SELECT 전용 제한, 위험 키워드 필터링
4. **Natural Language to SQL**: 자연어 질문을 SQL 로 변환
5. **데이터 기반 분석**: 통계 및 리텐션 분석 구현

---

## 📁 프로젝트 구성

```
Stage06_Database/
├── Stage06.sln
├── 06A_PlayerStatsBot/            # 플레이어 통계 조회 봇
│   ├── Program.cs
│   └── Tools/
│       └── SqlQueryTool.cs        # SQL 쿼리 실행 도구
├── 06B_RetentionAnalyzer/         # 리텐션 분석 에이전트
│   ├── Program.cs
│   └── Tools/
│       └── RetentionCalcTool.cs   # 리텐션 계산 도구
└── 06C_NaturalLanguageSql/        # 자연어 SQL 변환기
    ├── Program.cs
    ├── Tools/
    │   └── SqlGeneratorTool.cs    # NL-to-SQL 생성 도구
    └── Services/
        └── QueryValidator.cs      # SQL 검증 서비스
```

---

## 🔧 각 프로젝트 설명

### 06A_PlayerStatsBot - 플레이어 통계 조회 봇

**학습 내용:**
- SQLite 데이터베이스 연동
- 파라미터화된 쿼리로 SQL 인젝션 방지
- 안전한 데이터 조회 패턴

**사용 가능한 도구:**
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `GetPlayerStats` | 특정 플레이어 통계 조회 | `playerId: string` |
| `GetTopPlayers` | 상위 N 명 플레이어 조회 | `limit: int` |
| `GetPlayerCount` | 총 플레이어 수 조회 | 없음 |

**Players 테이블 스키마:**
```sql
CREATE TABLE Players (
    PlayerId TEXT PRIMARY KEY,
    PlayerName TEXT NOT NULL,
    Level INTEGER NOT NULL,
    TotalPlayTime REAL NOT NULL,
    LastLoginDate TEXT NOT NULL
)
```

**예시 요청:**
- "플레이어 1 의 통계를 보여줘"
- "상위 5 명 플레이어가 누구야?"
- "전체 플레이어 수가 얼마나 돼?"

**보안 패턴 (SQL 인젝션 방지):**
```csharp
// ❌ 위험: 사용자 입력 직접 사용
var sql = $"SELECT * FROM Players WHERE PlayerId = '{userInput}'";

// ✅ 안전: 파라미터화된 쿼리
var sql = "SELECT * FROM Players WHERE PlayerId = @PlayerId";
cmd.Parameters.AddWithValue("@PlayerId", userInput);
```

---

### 06B_RetentionAnalyzer - 리텐션 분석 에이전트

**학습 내용:**
- 일별/주별 리텐션 계산 SQL
- 날짜 함수 활용 (DATE, STRFTIME)
- 활성/비활성 사용자 분류

**사용 가능한 도구:**
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `CalculateDailyRetention` | 일별 리텐션 계산 | `days: int` |
| `CalculateWeeklyRetention` | 주별 리텐션 계산 | `weeks: int` |
| `GetActiveInactivePlayers` | 활성도 분석 | `inactiveDays: int` |

**분석 기준:**
- **활성 플레이어**: 30 일 이내 로그인
- **비활성 플레이어**: 30 일 이상 로그인 없음
- **리텐션율 50% 이상**: 양호
- **리텐션율 50% 미만**: 개선 필요

**예시 요청:**
- "최근 7 일 리텐션률을 계산해줘"
- "주별 사용자 추이를 보여줘"
- "비활성 플레이어가 몇 명이나 돼?"

**SQL 예시 (주별 리텐션):**
```sql
SELECT 
    STRFTIME('%Y-W%W', LastLoginDate) as Week,
    COUNT(DISTINCT PlayerId) as WeeklyUsers
FROM Players
GROUP BY STRFTIME('%Y-W%W', LastLoginDate)
ORDER BY Week DESC
LIMIT 4
```

---

### 06C_NaturalLanguageSql - 자연어 SQL 변환기

**학습 내용:**
- 자연어를 SQL 로 변환하는 패턴
- SQL 안전성 검증 (SELECT 전용 제한)
- 위험 키워드 필터링

**사용 가능한 도구:**
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `GenerateSql` | 자연어 → SQL 변환 | `naturalLanguage: string` |
| `ValidateSql` | SQL 안전성 검증 | `sql: string` |

**보안 가이드라인:**
- ✅ 허용: `SELECT` 쿼리만
- ❌ 차단: `DROP`, `DELETE`, `UPDATE`, `INSERT`, `ALTER`, `TRUNCATE`
- ✅ 제한: `LIMIT` 필수 (최대 1000 행)

**예시 요청:**
- "플레이어가 총 몇 명이야?" → `SELECT COUNT(*) FROM Players`
- "레벨이 가장 높은 플레이어 5 명을 보여줘" → `SELECT PlayerName, Level FROM Players ORDER BY Level DESC LIMIT 5`
- "플레이어들의 평균 레벨은?" → `SELECT AVG(Level) FROM Players`

**자연어 → SQL 매핑 예시:**
| 자연어 | 생성된 SQL |
|--------|-----------|
| "플레이어 수" | `SELECT COUNT(*) FROM Players` |
| "상위 레벨" | `SELECT PlayerName, Level ORDER BY Level DESC LIMIT 10` |
| "평균 레벨" | `SELECT AVG(Level) FROM Players` |

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-openai-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 2. 프로젝트 실행

```bash
# 06A 실행 (샘플 DB 자동 생성)
dotnet run --project 06A_PlayerStatsBot

# 06B 실행 (06A 의 DB 사용)
dotnet run --project 06B_RetentionAnalyzer

# 06C 실행
dotnet run --project 06C_NaturalLanguageSql
```

> **참고**: 06B_RetentionAnalyzer 를 실행하기 전에 06A_PlayerStatsBot을 먼저 실행하여 샘플 데이터베이스를 생성해야 합니다.

---

## 💡 핵심 개념

### 1. SQLite 연결 패턴

```csharp
using Microsoft.Data.Sqlite;

public class SqlQueryTool
{
    private readonly string _connectionString;

    public SqlQueryTool(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public string GetPlayerStats(string playerId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "SELECT * FROM Players WHERE PlayerId = @PlayerId";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);

        using var reader = cmd.ExecuteReader();
        // 결과 처리
    }
}
```

### 2. SQL 인젝션 방어 기법

| 공격 패턴 | 방어 방법 |
|-----------|-----------|
| `' OR '1'='1` | 파라미터화된 쿼리 사용 |
| `'; DROP TABLE Players; --` | 입력 값 이스케이프 |
| `UNION SELECT password FROM Users` | 쿼리 검증 (SELECT 전용) |

### 3. SQL 검증 패턴

```csharp
public class QueryValidator
{
    public bool IsReadOnly(string sql)
    {
        var writeKeywords = new[] { "INSERT", "UPDATE", "DELETE", "DROP" };
        
        foreach (var keyword in writeKeywords)
        {
            if (sql.ToUpper().StartsWith(keyword))
            {
                return false; // 쓰기 쿼리 차단
            }
        }
        
        return true; // 읽기 전용 허용
    }
}
```

### 4. 날짜 함수 (SQLite)

| 함수 | 설명 | 예시 |
|------|------|------|
| `DATE(column)` | 날짜 추출 | `DATE(LastLoginDate)` |
| `STRFTIME(format, column)` | 포맷팅 | `STRFTIME('%Y-W%W', LastLoginDate)` |
| `DATE('now', '-7 days')` | 상대 날짜 | 7 일 전 날짜 |

---

## ⚠️ 보안 주의사항

### 1. SQL 인젝션 항상 방지

```csharp
// ❌ 절대 사용하지 마세요
var sql = $"SELECT * FROM Users WHERE Name = '{userInput}'";

// ✅ 항상 파라미터 사용
var sql = "SELECT * FROM Users WHERE Name = @Name";
cmd.Parameters.AddWithValue("@Name", userInput);
```

### 2. 최소 권한 원칙

- 게임 서버 DB 는 **읽기 전용** 계정 사용
- `DROP`, `DELETE`, `ALTER` 권한 제거
- `LIMIT`로 결과 행 수 제한

### 3. 입력 검증

```csharp
// 정수 입력 검증
if (!int.TryParse(input, out var playerId))
{
    return "유효하지 않은 ID 입니다.";
}

// 문자열 길이 제한
if (playerName.Length > 50)
{
    return "이름이 너무 깁니다.";
}
```

---

## 📝 연습 과제

1. **페이지네이션 구현**: `OFFSET` 을 사용한 페이지별 조회
2. **집계 함수 확장**: `SUM`, `AVG`, `MAX`, `MIN` 활용
3. **조인 쿼리**: 여러 테이블 조인하여 복잡한 조회
4. **트랜잭션**: 여러 쿼리를 원자적으로 실행
5. **인덱스**: 성능 향상을 위한 인덱스 추가

---

## 🔗 다음 단계

- **Stage 07_Planning**: 복잡한 작업 계획 수립 (ReAct 패턴)
- **Stage 08_MultiAgent**: 여러 Agent 간 협업
- **Stage 09_Memory**: 장기 메모리 및 컨텍스트 관리

---

## 📚 참고 문서

- [SQLite 공식 문서](https://www.sqlite.org/docs.html)
- [Microsoft.Data.Sqlite NuGet](https://www.nuget.org/packages/Microsoft.Data.Sqlite)
- [SQL 인젝션 방어 가이드 (OWASP)](https://owasp.org/www-community/attacks/SQL_Injection)
- [Natural Language to SQL 패턴](https://learn.microsoft.com/en-us/agent-framework/)
