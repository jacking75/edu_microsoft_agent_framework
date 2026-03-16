# SkillSystem: 마크다운 스킬 기반 에이전트

> **학습 목표**: 마크다운 파일로 스킬을 정의하고, **코드 수정 없이** 에이전트 기능을 동적으로 확장하기

---

## 🎯 이 프로젝트에서 배우는 것

### 핵심 학습 목표

1. **마크다운 스킬 정의**
   - 스킬 이름, 설명, 함수, 파라미터를 마크다운으로 정의
   - 코드 변경 없이 스킬 추가/수정 가능

2. **범용 Tool 활용**
   - CommandExecutor: CMD 명령어 실행
   - FileDownloader: 인터넷 파일 다운로드
   - PackageManager: winget 패키지 관리
   - WebSearcher: 웹 검색
   - FileHandler: 파일 읽기/쓰기/삭제

3. **스킬 파서 구현**
   - 마크다운 파일을 읽어서 스킬 정의 추출
   - 함수 시그니처와 파라미터 정보 추출

4. **동적 Tool 등록**
   - 로드된 스킬을 기반으로 Tool 자동 생성
   - Agent 에 동적으로 Tool 등록

5. **스킬 확장성**
   - 새로운 스킬 디렉토리 추가만으로 기능 확장
   - 기존 코드 수정 불필요

---

## 📁 프로젝트 구성

```
SkillSystem/
├── Program.cs                 # 메인 실행 파일
├── SkillLoader.cs             # 마크다운 스킬 파일 파서
├── SkillDefinition.cs         # 스킬 데이터 모델
├── SkillTools.cs              # 전용 스킬 함수 (계산, 번역, 날씨)
├── GenericTools.cs            # 범용 Tool (명령어, 파일, 웹 등)
├── SkillSystem.csproj         # .NET 10 프로젝트 파일
├── README.md                  # 사용 가이드
└── skills/                    # 스킬 정의 디렉토리
    ├── Calculator/
    │   └── skill.md           # 계산기 스킬 (3 함수)
    ├── Translator/
    │   └── skill.md           # 번역 스킬 (3 함수)
    ├── Weather/
    │   └── skill.md           # 날씨 스킬 (3 함수)
    ├── SystemTools/
    │   └── skill.md           # 시스템 도구 (12 함수)
    └── WebTools/
        └── skill.md           # 웹 도구 (5 함수)
```

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 2. NuGet 패키지 복원

```bash
cd src/SkillSystem
dotnet restore
```

### 3. 프로젝트 실행

```bash
cd src/SkillSystem
dotnet run
```

### 4. 실행 결과 예시

```
🎯 Skill-based Agent System
==========================

📚 스킬 로딩 중...

✅ 스킬 로드 완료: Calculator
✅ 스킬 로드 완료: SystemTools
✅ 스킬 로드 완료: Translator
✅ 스킬 로드 완료: Weather
✅ 스킬 로드 완료: WebTools

✅ 총 5 개의 스킬을 로드했습니다.

  📌 Calculator: 3 개 함수
    - calculate_dps: 무기의 초당 데미지 (DPS) 를 계산합니다.
    - calculate_crit_damage: 크리티컬 발생 시 입히는 데미지를 계산합니다.
    - calculate_expected_dps: 크리티컬 확률을 고려한 기대 DPS 를 계산합니다.
  📌 SystemTools: 12 개 함수
    - execute_command: CMD 명령어를 실행하고 결과를 반환합니다.
    - download_file: 인터넷에서 파일을 다운로드합니다.
    - install_package: winget 을 사용하여 패키지를 설치합니다.
    - get_system_info: 시스템 기본 정보를 가져옵니다.
    - get_process_list: 실행 중인 프로세스 목록을 가져옵니다.
    - ...
  📌 Translator: 3 개 함수
    - translate_ko_to_ja: 한국어 텍스트를 일본어로 번역합니다.
    - translate_ja_to_ko: 일본어 텍스트를 한국어로 번역합니다.
    - translate_game_term: 게임 용어를 번역합니다.
  📌 Weather: 3 개 함수
    - get_current_weather: 현재 지역의 날씨 정보를 조회합니다.
    - get_weekend_forecast: 주말 날씨 예보를 조회합니다.
    - compare_weather: 두 지역의 날씨를 비교합니다.
  📌 WebTools: 5 개 함수
    - fetch_url: 웹 페이지의 내용을 가져옵니다.
    - search_google: Google 에서 검색을 수행합니다.
    - ...

🔧 총 26 개의 Tool 이 등록되었습니다.

💬 에이전트가 준비되었습니다. 질문을 입력하세요 (종료: 'quit')
```

---

## 🎮 실행해 볼 것 - 추천 미션

### Calculator 스킬 테스트

**기본 미션:**
- [ ] "공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘"
- [ ] "기본 데미지 200 이 크리티컬로 맞으면 몇이야? (배율 2.5)"
- [ ] "DPS 150, 크리티컬 확률 30%, 배율 2 배일 때 기대 DPS 는?"

**예상 응답:**
```
👤 사용자: 공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘
🤖 에이전트: 무기의 DPS 는 150 입니다. (수식: 100 × 1.5 = 150)

👤 사용자: DPS 150, 크리티컬 확률 30%, 배율 2 배일 때 기대 DPS 는?
🤖 에이전트: 크리티컬 확률을 고려한 기대 DPS 는 195 입니다.
             (수식: 150 × (1 + 0.3 × (2 - 1)) = 150 × 1.3 = 195)
```

**배우는 것:**
- ✅ 마크다운 스킬 정의가 Tool 로 자동 변환
- ✅ 파라미터 설명이 Agent 의 이해도 향상
- ✅ 계산 결과와 수식 함께 제공

---

### SystemTools 스킬 테스트 (범용 Tool)

**시스템 명령어:**
- [ ] "현재 디렉토리 목록을 보여줘"
- [ ] "systeminfo 명령어를 실행해줘"
- [ ] "실행 중인 프로세스 목록을 보여줘"

**파일 조작:**
- [ ] "test.txt 파일에 'Hello World' 라고 써줘"
- [ ] "test.txt 파일 내용을 읽어줘"
- [ ] "test.txt 파일을 삭제해줘"

**패키지 관리:**
- [ ] "VSCode 를 검색해줘"
- [ ] "Notepad++ 를 설치해줘"

**웹 기능:**
- [ ] "Google 에서 'C# 학습' 을 검색해줘"
- [ ] "https://www.google.com 페이지 내용을 가져와줘"
- [ ] "https://example.com/file.txt 파일을 다운로드해줘"

**예상 응답:**
```
👤 사용자: 현재 디렉토리 목록을 보여줘
🤖 에이전트: 📁 디렉토리: F:\github_edu\edu_microsoft_agent_framework\src\SkillSystem

             📂 폴더 (3 개):
               bin/
               obj/
               skills/

             📄 파일 (5 개):
               Program.cs
               SkillDefinition.cs
               SkillLoader.cs
               GenericTools.cs
               SkillTools.cs

👤 사용자: test.txt 파일에 'Hello World' 라고 써줘
🤖 에이전트: ✅ 파일 작성 완료: test.txt (11 bytes)

👤 사용자: VSCode 를 검색해줘
🤖 에이전트: winget 검색 결과:
             Name      ID                Version
             -----------------------------------------
             VSCode    Microsoft.VSCode  1.85.0
```

**배우는 것:**
- ✅ 범용 Tool 을 사용한 다양한 기능 구현
- ✅ 코드 수정 없이 스킬 파일만으로 기능 추가
- ✅ 보안 고려사항 (위험한 명령어 차단)

---

### Weather 스킬 테스트

**기본 미션:**
- [ ] "서울 현재 날씨가 어때?"
- [ ] "제주도 주말 날씨 예보 알려줘"
- [ ] "부산과 도쿄 날씨 비교해줘"

**예상 응답:**
```
👤 사용자: 서울 현재 날씨가 어때?
🤖 에이전트: 서울 현재 날씨: 맑음, 23°C, 미세먼지 보통

👤 사용자: 제주도 주말 날씨 예보 알려줘
🤖 에이전트: 제주 주말 예보:
             토요일: 비, 22°C / 일요일: 흐림, 23°C
```

**배우는 것:**
- ✅ 가상 데이터 조회 스킬 구현
- ✅複数 파라미터 처리
- ✅ 비교 분석 기능

---

### Translator 스킬 테스트

**기본 미션:**
- [ ] "안녕하세요를 일본어로 번역해줘"
- [ ] "こんにちは를 한국어로 번역해줘"
- [ ] "공격력이라는 게임 용어를 일본어로 어떻게 써?"

**예상 응답:**
```
👤 사용자: 안녕하세요를 일본어로 번역해줘
🤖 에이전트: 안녕하세요 → こんにちは

👤 사용자: 공격력이라는 게임 용어를 일본어로 어떻게 써?
🤖 에이전트: 공격력 → 攻撃力 (こうげきりょく)
```

**배우는 것:**
- ✅ 언어 변환 스킬 구현
- ✅ 딕셔너리 기반 번역
- ✅ 다국어 지원 구조

---

### WebTools 스킬 테스트

**기본 미션:**
- [ ] "https://www.google.com 페이지를 가져와줘"
- [ ] ".NET 10 신기능을 Google 에서 검색해줘"
- [ ] "Microsoft Agent Framework 를 GitHub 에서 찾아줘"
- [ ] "google.com 이 정상 작동하는지 확인해줘"

**예상 응답:**
```
👤 사용자: google.com 이 정상 작동하는지 확인해줘
🤖 에이전트: ✅ https://google.com 이 (가) 정상 응답합니다.
             상태 코드: 200 OK
             응답 시간: 2024-03-13 오전 10:30:45

👤 사용자: Microsoft Agent Framework 를 GitHub 에서 찾아줘
🤖 에이전트: 🌐 'site:github.com Microsoft Agent Framework' (으) 로 웹 검색을 시작합니다.
             브라우저가 열립니다: https://www.google.com/search?q=site:github.com+...
```

**배우는 것:**
- ✅ 웹 API 호출
- ✅ 상태 코드 확인
- ✅ 브라우저 연동

---

## 📖 스킬 마크다운 파일 형식

### 기본 구조

```markdown
# Skill: 스킬이름

## Description
스킬에 대한 설명을 작성합니다.

## Functions

### 함수이름
- **설명**: 함수의 기능을 설명합니다.
- **파라미터**:
- param1: type (파라미터 설명)
- param2: type (파라미터 설명)
- **수식**: 수행할 계산식 (선택사항)

## Examples
- "예시 질문 1"
- "예시 질문 2"
```

### 예시 1: Calculator 스킬

```markdown
# Skill: Calculator

## Description
게임 밸런스 계산을 수행하는 스킬입니다.

## Functions

### calculate_dps
- **설명**: 무기의 초당 데미지 (DPS) 를 계산합니다.
- **파라미터**:
- damage: int (무기의 기본 공격력)
- attacksPerSecond: double (초당 공격 속도)
- **수식**: damage * attacksPerSecond

## Examples
- "공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘"
```

### 예시 2: SystemTools 스킬 (범용 Tool)

```markdown
# Skill: SystemTools

## Description
시스템 관리, 파일 조작, 패키지 설치 등의 작업을 수행하는 스킬입니다.

## Functions

### execute_command
- **설명**: CMD 명령어를 실행하고 결과를 반환합니다.
- **파라미터**:
- command: string (실행할 명령어)
- **수식**: cmd.exe /c {command}
- **주의**: del, format, shutdown 등 위험한 명령어는 차단됩니다.

### download_file
- **설명**: 인터넷에서 파일을 다운로드합니다.
- **파라미터**:
- url: string (다운로드할 파일 URL)
- savePath: string (저장할 경로, 선택사항)

### list_directory
- **설명**: 디렉토리 목록을 조회합니다.
- **파라미터**:
- path: string (조회할 디렉토리 경로, 선택사항)

## Examples
- "현재 디렉토리 목록을 보여줘"
- "https://example.com/file.txt 파일을 다운로드해줘"
- "test.txt 파일에 'Hello' 라고 써줘"
```

### 예시 3: WebTools 스킬

```markdown
# Skill: WebTools

## Description
웹 관련 작업을 수행하는 스킬입니다.

## Functions

### fetch_url
- **설명**: 웹 페이지의 내용을 가져옵니다.
- **파라미터**:
- url: string (웹 페이지 URL)

### search_google
- **설명**: Google 에서 검색을 수행합니다.
- **파라미터**:
- query: string (검색어)

### check_website_status
- **설명**: 웹사이트가 응답하는지 확인합니다.
- **파라미터**:
- url: string (확인할 웹사이트 URL)

## Examples
- "https://www.google.com 페이지를 가져와줘"
- "google.com 이 정상 작동하는지 확인해줘"
```

---

## 🔧 새 스킬 추가 방법 (코드 수정 없이!)

### 1. 새 디렉토리 생성

```bash
mkdir skills/MyNewSkill
```

### 2. skill.md 파일 생성

```markdown
# Skill: MyNewSkill

## Description
새로운 스킬의 설명을 작성합니다.

## Functions

### my_function
- **설명**: 이 함수의 기능을 설명합니다.
- **파라미터**:
- input: string (입력 값)
- **수식**: 수행할 작업

## Examples
- "예시 질문"
```

### 3. Program.cs 에 Tool 매핑 추가

**⚠️ 중요**: 범용 Tool 을 사용하는 경우 Program.cs 수정이 필요합니다.  
하지만 **새로운 범용 Tool 함수**를 추가하는 것만으로 기존 스킬은 수정 없이 사용 가능합니다.

```csharp
// GenericTools.cs 에 새 함수 추가 후
// Program.cs 의 CreateToolForFunction 에서 매핑
"my_function" => AIFunctionFactory.Create(genericTools.MyFunction),
```

### 4. 실행!

코드 수정 없이 `skill.md` 파일만 추가해도 에이전트가 자동으로 인식합니다.

---

## 💡 핵심 개념

### 1. 스킬 vs Tool

| 개념 | 설명 | 위치 |
|------|------|------|
| **스킬** | 마크다운 파일로 정의된 기능 명세 | `skills/*/skill.md` |
| **Tool** | C# 코드로 구현된 실제 함수 | `GenericTools.cs`, `SkillTools.cs` |
| **매핑** | 스킬 이름을 Tool 함수와 연결 | `Program.cs` |

### 2. 동작 과정

```
1. 프로그램 시작
   ↓
2. skills/ 디렉토리 스캔
   ↓
3. skill.md 파일 파싱
   ↓
4. 스킬 정의 추출 (이름, 함수, 파라미터)
   ↓
5. C# Tool 과 매핑
   ↓
6. Agent 에 Tool 등록
   ↓
7. 사용자 질문에 따라 자동 Tool 호출
```

### 3. 범용 Tool 의 장점

**before (기존 방식):**
- 스킬마다 C# 코드 추가 필요
- 계산기 스킬 → CalculateDps() 함수 추가
- 날씨 스킬 → GetWeather() 함수 추가
- 번역 스킬 → Translate() 함수 추가

**after (범용 Tool 방식):**
- **SystemTools 스킬**: GenericTools 의 12 개 함수 사용
- **WebTools 스킬**: GenericTools 의 5 개 함수 사용
- **새로운 스킬**: 기존 범용 Tool 조합으로 대부분 구현 가능

**예시:**
```markdown
# Skill: GameDevTools

## Functions
### execute_command    ← GenericTools.ExecuteCommand 사용
### download_file      ← GenericTools.DownloadFile 사용
### read_file          ← GenericTools.ReadFile 사용
```

이렇게 정의하면 **코드 수정 없이** 마크다운 파일만으로 새 스킬 추가 가능!

---

## 📊 스킬 정의 예시 (완전 본물)

### Calculator 스킬 (skills/Calculator/skill.md)

```markdown
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

### calculate_expected_dps
- **설명**: 크리티컬 확률을 고려한 기대 DPS 를 계산합니다.
- **파라미터**:
- dps: double (기본 DPS)
- critChance: double (크리티컬 확률, 0-1 범위)
- critMultiplier: double (크리티컬 배율)
- **수식**: dps * (1 + (critChance * (critMultiplier - 1)))

## Examples
- "공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘"
- "DPS 150, 크리티컬 확률 30%, 배율 2 배일 때 기대 DPS 는?"
```

### SystemTools 스킬 (skills/SystemTools/skill.md)

```markdown
# Skill: SystemTools

## Description
시스템 관리, 파일 조작, 패키지 설치 등의 작업을 수행하는 스킬입니다.
명령어 실행, 파일 다운로드, winget 패키지 관리, 웹 검색 등 다양한 기능을 제공합니다.

## Functions

### execute_command
- **설명**: CMD 명령어를 실행하고 결과를 반환합니다.
- **파라미터**:
- command: string (실행할 명령어)
- **수식**: cmd.exe /c {command}
- **주의**: del, format, shutdown 등 위험한 명령어는 차단됩니다.

### download_file
- **설명**: 인터넷에서 파일을 다운로드합니다.
- **파라미터**:
- url: string (다운로드할 파일 URL)
- savePath: string (저장할 경로, 선택사항)
- **수식**: HTTP GET 요청으로 파일 다운로드

### list_directory
- **설명**: 디렉토리 목록을 조회합니다.
- **파라미터**:
- path: string (조회할 디렉토리 경로, 선택사항. 생략 시 현재 디렉토리)
- **수식**: Directory.GetDirectories/GetFiles()

## Examples
- "현재 디렉토리 목록을 보여줘"
- "systeminfo 명령어를 실행해줘"
- "https://example.com/file.txt 파일을 다운로드해줘"
```

---

## 🔍 스킬 파서 동작 원리

### 마크다운 파싱 로직

```csharp
public class SkillLoader
{
    public List<SkillDefinition> LoadAllSkills()
    {
        // 1. skills/ 디렉토리 하위 모든 폴더 스캔
        var skillDirs = Directory.GetDirectories(_skillsDirectory);
        
        foreach (var dir in skillDirs)
        {
            // 2. skill.md 파일 찾기
            var skillFile = Path.Combine(dir, "skill.md");
            if (File.Exists(skillFile))
            {
                // 3. 마크다운 파싱
                var skill = ParseSkillFile(skillFile);
                skills.Add(skill);
            }
        }
        
        return skills;
    }
}
```

### 파싱되는 항목

1. **스킬 이름**: `# Skill: 이름`
2. **설명**: `## Description` 섹션
3. **함수 목록**: `## Functions` 섹션
   - 함수 이름: `### 함수이름`
   - 함수 설명: `- **설명**:`
   - 파라미터: `- **파라미터**:`
   - 수식: `- **수식**:`
4. **예시**: `## Examples` 섹션

---

## 🎯 스킬 설계 모범 사례

### ✅ 좋은 스킬 정의

```markdown
### calculate_dps
- **설명**: 무기의 초당 데미지 (DPS) 를 계산합니다.
- **파라미터**: 
  - damage: int (무기의 기본 공격력)
  - attacksPerSecond: double (초당 공격 속도)
- **수식**: damage * attacksPerSecond
```

**좋은 점:**
- ✅ 명확한 함수 이름 (동사 + 명사)
- ✅ 구체적인 설명 (무엇을 계산하는지)
- ✅ 파라미터 타입과 설명 명시
- ✅ 수식 표시 (Agent 가 설명하기 쉬움)

### ❌ 나쁜 스킬 정의

```markdown
### calc
- **설명**: 계산을 합니다.
- **파라미터**: a, b
```

**문제점:**
- ❌ 모호한 함수 이름
- ❌ 너무 일반적인 설명
- ❌ 파라미터 타입 없음
- ❌ 어떤 계산인지 불명확

---

## 📝 학습 체크리스트

### 기본 개념
- [ ] 스킬 (마크다운) 과 Tool(C# 코드) 의 차이 이해
- [ ] 스킬 디렉토리 구조 이해
- [ ] 마크다운 파일 형식 숙지

### 실습
- [ ] Calculator 스킬 테스트 (DPS 계산)
- [ ] Weather 스킬 테스트 (날씨 조회)
- [ ] Translator 스킬 테스트 (번역)
- [ ] 새로운 스킬 추가해보기

### 심화 이해
- [ ] 스킬 파서 동작 원리 이해
- [ ] Tool 매핑 과정 이해
- [ ] 스킬 확장 방법 숙지
- [ ] 마크다운 수정으로 기능 추가 경험

---

## 🔗 관련 문서

- **[Microsoft Agent Framework 공식 문서](https://learn.microsoft.com/agent-framework/)**
- **[Function Tools 가이드](https://learn.microsoft.com/agent-framework/agents/tools/function-tools)**
- **[GitHub Repository](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples)**

---

## 📌 NuGet 패키지

```xml
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc1" />
<PackageReference Include="Microsoft.Extensions.AI" Version="10.3.0" />
<PackageReference Include="System.ClientModel" Version="1.8.1" />
```

---

## 🎯 다음 단계

SkillSystem 을 마스터했다면:

1. **새로운 스킬 추가**: 자신만의 스킬을 만들어보세요
2. **범용 Tool 확장**: 새로운 GenericTools 함수를 추가해보세요
3. **Stage 3 으로**: 파일 시스템 연동 에이전트 학습
4. **Stage 4 로**: 다중 Tool 활용 에이전트 학습

---

## 💡 팁

### 스킬 디버깅

프로그램 시작 시 출력되는 스킬 로드 메시지를 확인하세요:

```
✅ 스킬 로드 완료: Calculator
✅ 스킬 로드 완료: SystemTools
✅ 스킬 로드 완료: Translator
✅ 스킬 로드 완료: Weather
✅ 스킬 로드 완료: WebTools

✅ 총 5 개의 스킬을 로드했습니다.

  📌 Calculator: 3 개 함수
  📌 SystemTools: 12 개 함수
  📌 Translator: 3 개 함수
  📌 Weather: 3 개 함수
  📌 WebTools: 5 개 함수

🔧 총 26 개의 Tool 이 등록되었습니다.
```

### 스킬이 인식되지 않을 때

1. `skills/` 디렉토리 구조 확인
2. 파일 이름이 `skill.md` 인지 확인
3. 마크다운 형식이 올바른지 확인
4. `# Skill: 이름` 헤더가 있는지 확인

### 범용 Tool 활용하기

새로운 스킬을 만들 때 **먼저 범용 Tool 조합**으로 가능한지 생각해보세요:

- ✅ 명령어 실행: `execute_command`
- ✅ 파일 조작: `read_file`, `write_file`, `delete_file`
- ✅ 디렉토리 조회: `list_directory`
- ✅ 웹 검색: `search_web`
- ✅ 파일 다운로드: `download_file`

이렇게 하면 **코드 수정 없이** 마크다운 파일만으로 새 스킬 추가 가능!
