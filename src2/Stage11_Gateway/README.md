# Stage 11 - Gateway & Deployment

Microsoft Agent Framework 기반의 프로덕션 레디 게이트웨이입니다.

## 📋 주요 기능

| 기능 | 설명 |
|------|------|
| **Health Checks** | `/health`, `/health/ready` - 서비스 상태 확인 |
| **Structured Logging** | Serilog 기반 일관된 로그 |
| **REST API** | `/api/chat` - AI 채팅 엔드포인트 |
| **Streaming** | `/api/chat/stream` - 실시간 스트리밍 |
| **Docker** | 컨테이너 배포 지원 |
| **CORS** | 크로스 오리진 요청 허용 |

## 🚀 실행 방법

### 로컬 실행

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="sk-..."
$env:OPENAI_MODEL="gpt-4o-mini"

# 실행
dotnet run --project scr2/Stage11_Gateway
```

### Docker 실행

```bash
# Docker 이미지 빌드
docker build -t stage11-gateway ./scr2/Stage11_Gateway

# Docker 컨테이너 실행
docker run -d -p 8080:8080 \
  -e OPENAI_API_KEY=sk-... \
  -e OPENAI_MODEL=gpt-4o-mini \
  --name gateway \
  stage11-gateway
```

### Kubernetes 배포 (예시)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gateway
spec:
  replicas: 3
  selector:
    matchLabels:
      app: gateway
  template:
    metadata:
      labels:
        app: gateway
    spec:
      containers:
      - name: gateway
        image: stage11-gateway:latest
        ports:
        - containerPort: 8080
        env:
        - name: OPENAI_API_KEY
          valueFrom:
            secretKeyRef:
              name: openai-secret
              key: api-key
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: gateway
spec:
  selector:
    app: gateway
  ports:
  - port: 80
    targetPort: 8080
  type: LoadBalancer
```

## 📡 API 엔드포인트

### `GET /` - 서비스 정보

```bash
curl http://localhost:5000
```

응답:
```json
{
  "service": "Stage 11 Gateway",
  "status": "running",
  "endpoints": [
    "GET  /                  - Service info",
    "GET  /health            - Health check (all checks)",
    "GET  /health/ready      - Readiness probe",
    "POST /api/chat          - Chat with AI",
    "POST /api/chat/stream   - Streaming chat"
  ]
}
```

### `GET /health` - 헬스체크

```bash
curl http://localhost:5000/health
```

응답 (정상):
```json
{
  "status": "Healthy"
}
```

### `POST /api/chat` - AI 채팅

```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "안녕하세요!", "maxTokens": 500}'
```

응답:
```json
{
  "message": "안녕하세요! 무엇을 도와드릴까요?",
  "timestamp": "2026-03-24T15:30:45.1234567+09:00",
  "model": "gpt-4o-mini"
}
```

### `POST /api/chat/stream` - 스트리밍 채팅

```bash
curl -X POST http://localhost:5000/api/chat/stream \
  -H "Content-Type: application/json" \
  -d '{"message": "1 부터 100 까지 더하면?"}'
```

## 📊 모니터링

### 로그 확인

```bash
# 콘솔 로그
docker logs -f gateway

# 파일 로그
tail -f logs/gateway-20260324.log
```

### 헬스체크 확인

```bash
# 전체 헬스체크
curl http://localhost:5000/health

# Readiness probe (K8s 용)
curl http://localhost:5000/health/ready
```

## 🔒 보안 설정

### 환경 변수로 기밀 정보 관리

```bash
# .env 파일 (gitignore 에 추가)
OPENAI_API_KEY=sk-...
ASPNETCORE_ENVIRONMENT=Production
```

### Docker Secrets (Swarm)

```bash
echo "sk-..." | docker secret create openai_key -
docker service create \
  --name gateway \
  --secret openai_key \
  -e OPENAI_API_KEY_FILE=/run/secrets/openai_key \
  stage11-gateway
```

## 📁 프로젝트 구조

```
Stage11_Gateway/
├── Program.cs              # 메인 애플리케이션
├── appsettings.json        # 설정 파일
├── Dockerfile             # Docker 설정
├── logs/                  # 로그 폴더 (gitignore)
│   └── gateway-YYYYMMDD.log
└── bin/                   # 빌드 출력 (gitignore)
```

## 🎯 프로덕션 체크리스트

- [x] Health Checks 구현
- [x] Structured Logging (Serilog)
- [x] Docker 컨테이너화
- [x] 환경 변수 기반 설정
- [x] CORS 설정
- [x] REST API 엔드포인트
- [x] 스트리밍 지원
- [ ] Rate Limiting (추가 필요)
- [ ] 인증/인가 (추가 필요)
- [ ] Distributed Tracing (추가 필요)

## 🛠️ 문제 해결

### 헬스체크 실패

```bash
# OpenAI API 키 확인
echo $OPENAI_API_KEY

# 컨테이너 로그 확인
docker logs gateway

# 내부에서 직접 테스트
docker exec -it gateway curl http://localhost:8080/health
```

### Docker 빌드 오류

```bash
# 캐시 삭제 후 재빌드
docker build --no-cache -t stage11-gateway ./scr2/Stage11_Gateway
```

## 📚 추가 리소스

- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Serilog Documentation](https://serilog.net/)
- [Docker Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)
- [Kubernetes Health Probes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
