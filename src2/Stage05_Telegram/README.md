# Stage 05 - Telegram Bot 연동

Microsoft Agent Framework 을 사용한 Telegram Bot 연동 예제입니다.

## 📋 사전 준비사항

### 1. Telegram Bot 생성

1. Telegram 에서 [@BotFather](https://t.me/BotFather) 검색
2. `/newbot` 명령 실행
3. 봇 이름과 사용자명 입력 (예: `MyAgentBot`, `my_test_agent_bot`)
4. 발급된 토큰 복사 (예: `123456789:ABCdefGHIjklMNOpqrsTUVwxyz`)

### 2. Webhook 설정 (로컬 테스트용)

로컬에서 테스트하려면 ngrok 같은 터널링 도구가 필요합니다.

```bash
# ngrok 설치 후 실행
ngrok http 5000
```

ngrok 에서 제공하는 HTTPS URL 을 메모해 둡니다 (예: `https://abc123.ngrok.io`)

### 3. Webhook 등록

```bash
curl -X POST "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/setWebhook?url=<YOUR_NGROK_URL>/telegram"
```

예시:
```bash
curl -X POST "https://api.telegram.org/bot123456789:ABCdefGHIjklMNOpqrsTUVwxyz/setWebhook?url=https://abc123.ngrok.io/telegram"
```

## 🚀 실행 방법

### 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="sk-..."
$env:OPENAI_MODEL="gpt-4o-mini"
$env:TELEGRAM_BOT_TOKEN="123456789:ABCdefGHIjklMNOpqrsTUVwxyz"

# 실행
dotnet run --project scr2/Stage05_Telegram
```

### Docker 사용 (선택사항)

```bash
docker build -t telegram-bot .
docker run -d -p 5000:80 \
  -e OPENAI_API_KEY=sk-... \
  -e TELEGRAM_BOT_TOKEN=your_token \
  telegram-bot
```

## 🔧 웹훅 확인 및 관리

```bash
# 웹훅 상태 확인
curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getWebhookInfo"

# 웹훅 삭제 (폴링 모드로 전환)
curl -X POST "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/deleteWebhook"

# 봇 정보 확인
curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getMe"
```

## 📡 API 엔드포인트

| 엔드포인트 | 메서드 | 설명 |
|-----------|--------|------|
| `/` | GET | 헬스체크 (상태 정보) |
| `/telegram` | POST | Telegram Webhook (자동 등록됨) |

## 💡 사용 예시

Telegram 봇에게 메시지를 보내면 AI 가 응답합니다:

```
User: 안녕하세요!
Bot: 안녕하세요! 무엇을 도와드릴까요?

User: 오늘 날씨가 어떨까?
Bot: 죄송하지만 실시간 날씨 정보에는 접근할 수 없습니다. 
     하지만 일반적인 날씨 정보를 알려드릴 수는 있어요!

User: 1 부터 10 까지 더하면?
Bot: 1 부터 10 까지의 합은 55 입니다!
```

## 🔒 보안 고려사항

1. **토큰 관리**: 봇 토큰을 git 에 커밋하지 마세요
2. **Webhook 보안**: Telegram IP 만 허용하도록 필터링 권장
3. **Rate Limiting**: 과도한 요청 제한 구현 권장

## 📝 추가 리소스

- [Telegram Bot API 문서](https://core.telegram.org/bots/api)
- [Telegram.Bot NuGet 패키지](https://www.nuget.org/packages/Telegram.Bot)
- [ngrok 다운로드](https://ngrok.com/download)

## ⚠️ 문제 해결

### 봇이 응답하지 않음
1. 웹훅 URL 이 올바른지 확인
2. 서버가 실행 중인지 확인
3. 봇 토큰이 유효한지 확인

### SSL 인증서 오류
- ngrok 사용 시 자동 해결됨
- 직접 인증서 사용 시 Let's Encrypt 권장

### Rate Limit 초과
- Telegram API 는 초당 약 30 메시지 제한
- Microsoft.Extensions.Http.Resilience 패키지로 재시도 로직 구현 권장
