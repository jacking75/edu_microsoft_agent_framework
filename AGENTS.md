# AGENTS.md - 이 저장소의 에이전트 코딩 가이드라인

최신 버전의 Micorsoft Agent Framework 를 아래 링크를 통해서 잘 확인해야한다.   
- [GitHub](https://github.com/microsoft/agent-framework )
- [Agent Framework documentation](https://learn.microsoft.com/en-us/agent-framework/ )    

## 저장소 개요
Microsoft Agent Framework 학습을 위한 교육용 저장소입니다.
현재는 커리큘럼 (README.md) 만 있으며, 코드는 15 단계 학습 경로에 따라 추가될 예정입니다.  

## 📋 핵심 개발 원칙

### 1. .NET 버전
- **반드시 .NET 10 사용**
- 모든 프로젝트는 `net10.0` 타겟

### 2. 단위 테스트
- **단위 테스트는 만들지 않는다**
- 교육용 코드 구현에 집중

### 3. 솔루션 파일 위치
- **솔루션 파일은 `src/` 디렉토리 안에 만든다**
- 예: `src/Stage01.sln`

### 4. NuGet 패키지
- **Microsoft.Agents.AI.OpenAI 사용** (공식 SDK)
- **Azure.AI.OpenAI 는 사용하지 않는다**
- **Azure.Identity 는 사용 가능** (인증을 위한)

## 빌드 명령어

```bash
# src 디렉토리에서 작업
cd src

# 빌드
dotnet build

# 코드 포매팅
dotnet format
```


## 예상 저장소 구조
```
edu_microsoft_agent_framework/
├── AGENTS.md
├── README.md
├── .opencode/
│   ├── skills/csharp-agent-framework.md   # 상세 코드 지침
│   └── agents/edu-agent.md                # 에이전트 정의
├── src/
│   ├── Stage01.sln                        # 솔루션 파일
│   ├── Stage01_TextAgent/                 # Stage 1 프로젝트
│   ├── Stage02_SingleTool/                # Stage 2 프로젝트
│   └── ...                                # 다른 Stage 들
└── samples/                               # 샘플 데이터
```

## AI 에이전트 참고사항
- **언어**: C#/.NET 전용
- **상세 코드 지침**: `.opencode/skills/csharp-agent-framework.md` 참조
- **LLM 설정**: OPENAI_BASE_URL, OPENAI_API_KEY 은 환경 설정을 사용하고 모델=gpt-5.4을 사용한다.
- **공식 문서**: https://learn.microsoft.com/en-us/agent-framework/
