# OpenClaw 학습 프로젝트 - Microsoft Agent Framework

OpenClaw 의 핵심 기능을 7~11 단계로 나누어 점진적으로 학습하는 프로젝트입니다.

각 단계는 이전 단계를 기반으로 기능이 추가되며, 작은 코드에서 큰 코드로 진화합니다.

**기술 스택:**
- C# / .NET 10
- Microsoft Agent Framework (최신 버전)
- NuGet: `Microsoft.Agents.AI.OpenAI`

---

## 📋 단계별 학습 로드맵

### [1 단계: Hello Agent](#1-단계-hello-agent)
**목표:** 가장 기본적인 Text Agent 만들기

**구현 기능:**
- Console 에서 사용자 입력 받기
- LLM 과 기본 대화
- 간단한 응답 출력

**학습 내용:**
- `Agent` 기본 클래스 사용법
- `AgentApplication` 설정
- OpenAI 모델 연결
- 기본 메시징 루프

**생성 프로젝트:**
- `Stage01_TextAgent/` - 콘솔 기반 텍스트 에이전트

---

### [2 단계: Tool 사용 - 단일 도구](#2-단계-tool-사용--단일-도구)
**목표:** Agent 에 도구 (Tool) 기능 추가

**구현 기능:**
- 날씨 조회 Tool 구현
- Tool 정의 및 등록
- Agent 가 Tool 자동 호출

**학습 내용:**
- `ToolAttribute` 데코레이터
- Tool 메서드 정의
- Tool 호출 프로토콜
- `ToolResult` 처리

**생성 프로젝트:**
- `Stage02_SingleTool/` - 단일 Tool 사용 에이전트

---

### [3 단계: Multi-Tool 지원](#3-단계-multi-tool-지원)
**목표:** 여러 도구 동시 지원

**구현 기능:**
- 날씨 조회, 시간 조회, 계산기 Tool
- Tool 선택 로직
- Tool 체이닝 (연속 호출)

**학습 내용:**
- 복수 Tool 등록
- Tool 우선순위 및 선택
- Tool 간 데이터 전달
- `ToolGroup` 개념

**생성 프로젝트:**
- `Stage03_MultiTool/` - 복수 Tool 사용 에이전트

---

### [4 단계: 채널 통합 - WebChat](#4-단계-채널-통합--webchat)
**목표:** Web 기반 채팅 인터페이스 연동

**구현 기능:**
- ASP.NET Core Web Server
- WebSocket 기반 실시간 통신
- 브라우저에서 채팅 UI

**학습 내용:**
- `ChannelController` 구현
- WebSocket 처리
- HTTP 엔드포인트 설정
- 프론트엔드 연동

**생성 프로젝트:**
- `Stage04_WebChat/` - Web 채팅 연동 에이전트

---

### [5 단계: 채널 통합 - 메신저 (Slack/Discord)](#5-단계-채널-통합--메신저slackdiscord)
**목표:** 실제 메신저 플랫폼 연동

**구현 기능:**
- Slack 또는 Discord 연동
- Bot 토큰 인증
- 메시지 수신/발신

**학습 내용:**
- 서드파티 API 연동
- Webhook 처리
- 인증 및 보안
- 이벤트 기반 아키텍처

**생성 프로젝트:**
- `Stage05_Messenger/` - 메신저 연동 에이전트

---

### [6 단계: 메모리 및 컨텍스트 관리](#6-단계-메모리-및-컨텍스트-관리)
**목표:** 대화 기록 및 상태 유지

**구형 기능:**
- 대화 히스토리 저장
- 세션별 컨텍스트 분리
- 메모리 프루닝 (요약)

**학습 내용:**
- `ITurnState` 관리
- `Memory` 서비스
- 컨텍스트 윈도우 최적화
- 영속화 전략

**생성 프로젝트:**
- `Stage06_Memory/` - 메모리 관리 에이전트

---

### [7 단계: Multi-Agent 라우팅](#7-단계-multi-agent-라우팅)
**목표:** 여러 에이전트 간 작업 분배

**구현 기능:**
- 루터 에이전트 (오케스트레이터)
- 특수화된 워커 에이전트
- 라우팅 로직

**학습 내용:**
- `Agent` 계층 구조
- 메시지 라우팅
- 에이전트 간 통신
- `Orchestration` 패턴

**생성 프로젝트:**
- `Stage07_MultiAgent/` - 멀티 에이전트 시스템

---

### [8 단계: 브라우저 자동화](#8-단계-브라우저-자동화)
**목표:** 웹 브라우저 제어 기능

**구현 기능:**
- Selenium/Playwright 연동
- 웹페이지 스크래핑
- 브라우저 스냅샷

**학습 내용:**
- 외부 프로세스 제어
- 비동기 작업 관리
- 스크린샷 처리
- 브라우저 Tool 구현

**생성 프로젝트:**
- `Stage08_Browser/` - 브라우저 자동화 에이전트

---

### [9 단계: Canvas UI 연동](#9-단계-canvas-ui-연동)
**목표:** 시각적 작업 공간 구현

**구현 기능:**
- 실시간 Canvas 업데이트
- UI 컴포넌트 렌더링
- 인터랙티브 요소

**학습 내용:**
- UI 상태 관리
- 실시간 스트리밍
- 프론트엔드 렌더링
- A2UI 개념

**생성 프로젝트:**
- `Stage09_Canvas/` - Canvas UI 연동 에이전트

---

### [10 단계: 음성 및 멀티모달](#10-단계-음성-및-멀티모달)
**목표:** 음성 입력/출력 및 미디어 처리

**구현 기능:**
- 음성 인식 (STT)
- 음성 합성 (TTS)
- 이미지/오디오 처리

**학습 내용:**
- 멀티모달 입력 처리
- 미디어 파이프라인
- 실시간 오디오 스트리밍
- Whisper/ElevenLabs 연동

**생성 프로젝트:**
- `Stage10_Voice/` - 음성 멀티모달 에이전트

---

### [11 단계: 게이트웨이 및 배포](#11-단계-게이트웨이-및-배포)
**목표:** 프로덕션 환경 구성

**구현 기능:**
- 게이트웨이 서버 구축
- Docker 컨테이너화
- Tailscale/SSH 원격 접근
- 헬스체크 및 모니터링

**학습 내용:**
- 서비스 아키텍처
- 컨테이너 배포
- 보안 설정
- 운영 모니터링

**생성 프로젝트:**
- `Stage11_Gateway/` - 프로덕션 게이트웨이

---

## 📁 프로젝트 구조

```
scr2/
├── README.md                          # 이 파일
├── Stage01_TextAgent/                 # 1 단계: 기본 텍스트 에이전트
├── Stage02_SingleTool/                # 2 단계: 단일 Tool
├── Stage03_MultiTool/                 # 3 단계: 복수 Tool
├── Stage04_WebChat/                   # 4 단계: WebChat 연동
├── Stage05_Messenger/                 # 5 단계: 메신저 연동
├── Stage06_Memory/                    # 6 단계: 메모리 관리
├── Stage07_MultiAgent/                # 7 단계: 멀티 에이전트
├── Stage08_Browser/                   # 8 단계: 브라우저 자동화
├── Stage09_Canvas/                    # 9 단계: Canvas UI
├── Stage10_Voice/                     # 10 단계: 음성 멀티모달
├── Stage11_Gateway/                   # 11 단계: 게이트웨이
└── Stage01.sln                        # 솔루션 파일
```

---

## 🚀 시작하기

### 1. 사전 요구사항
- .NET 10 SDK 설치
- Visual Studio 2022 이상 또는 VS Code
- OpenAI API 키

### 2. 환경 변수 설정
```bash
# Windows PowerShell
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
$env:OPENAI_API_KEY="your-api-key-here"

# 또는 .env 파일 생성
OPENAI_BASE_URL=https://api.openai.com/v1
OPENAI_API_KEY=your-api-key-here
```

### 3. 빌드 및 실행
```bash
cd scr2

# 전체 빌드
dotnet build

# 특정 단계 실행 (예: 1 단계)
dotnet run --project Stage01_TextAgent

# 코드 포매팅
dotnet format
```

---

## 📚 학습 자료

### 공식 문서
- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [GitHub Repository](https://github.com/microsoft/agent-framework)

### 참고 프로젝트
- [OpenClaw](https://github.com/openclaw/openclaw) - 개인용 AI 어시스턴트 (Node.js 기반)
- [AutoGen](https://microsoft.github.io/autogen/) - Microsoft AI Agent 프레임워크

---

## ✅ 단계별 완료 체크리스트

- [x] 1 단계: Hello Agent
- [x] 2 단계: Tool 사용 - 단일 도구
- [x] 3 단계: Multi-Tool 지원
- [x] 4 단계: 채널 통합 - WebChat
- [x] 5 단계: 채널 통합 - 메신저 (Telegram)
- [x] 6 단계: 메모리 및 컨텍스트 관리
- [x] 7 단계: Multi-Agent 라우팅
- [x] 8 단계: 브라우저 자동화
- [x] 9 단계: Canvas UI 연동
- [x] 10 단계: 음성 및 멀티모달
- [x] 11 단계: 게이트웨이 및 배포

---

## 📝 기여 가이드라인

1. 각 단계는 독립적으로 실행 가능해야 함
2. 이전 단계의 기능을 반드시 포함해야 함
3. 단위 테스트는 작성하지 않음 (교육용 집중)
4. 코드 컨벤션은 기존 프로젝트 따름
5. 주석은 최소화 (코드 자체로 이해 가능하게)

---

**마지막 업데이트:** 2026 년 3 월 24 일
