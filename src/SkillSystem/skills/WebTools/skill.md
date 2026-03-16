# Skill: WebTools

## Description
웹 관련 작업을 수행하는 스킬입니다.
URL에서 내용을 가져오거나, 파일을 다운로드받고, 웹 검색을 수행합니다.

## Functions

### fetch_url
- **설명**: 웹 페이지의 내용을 가져옵니다.
- **파라미터**:
- url: string (웹 페이지 URL)
- **수식**: HTTP GET 요청

### download_file_web
- **설명**: 인터넷에서 파일을 다운로드합니다.
- **파라미터**:
- url: string (파일 URL)
- filename: string (저장할 파일명, 선택사항)
- **수식**: HTTP GET 으로 바이너리 데이터 수신

### search_google
- **설명**: Google 에서 검색을 수행합니다.
- **파라미터**:
- query: string (검색어)
- **수식**: https://www.google.com/search?q={query}

### search_github
- **설명**: GitHub 에서 코드를 검색합니다.
- **파라미터**:
- query: string (검색어)
- **수식**: https://github.com/search?q={query}

### check_website_status
- **설명**: 웹사이트가 응답하는지 확인합니다.
- **파라미터**:
- url: string (확인할 웹사이트 URL)
- **수식**: HTTP HEAD 요청으로 상태 코드 확인

## Examples
- "https://www.google.com 페이지를 가져와줘"
- "https://example.com/data.json 파일을 받아와줘"
- ".NET 10 신기능을 Google 에서 검색해줘"
- "Microsoft Agent Framework 를 GitHub 에서 찾아줘"
- "google.com 이 정상 작동하는지 확인해줘"
