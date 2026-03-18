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

### install_package
- **설명**: winget 을 사용하여 패키지를 설치합니다.
- **파라미터**:
- packageName: string (패키지 ID 또는 이름)
- **수식**: winget install --id {packageName}

### search_package
- **설명**: winget 으로 패키지를 검색합니다.
- **파라미터**:
- packageName: string (검색할 패키지 이름)
- **수식**: winget search {packageName}

### get_system_info
- **설명**: 시스템 기본 정보를 가져옵니다.
- **파라미터**: 없음
- **수식**: systeminfo 명령어 실행

### get_process_list
- **설명**: 실행 중인 프로세스 목록을 가져옵니다.
- **파라미터**: 없음
- **수식**: tasklist 명령어 실행

### search_web
- **설명**: 웹 브라우저에서 검색을 수행합니다.
- **파라미터**:
- query: string (검색어)
- **수식**: Google 검색 URL 생성 및 브라우저 실행

### get_url_content
- **설명**: URL 의 웹 페이지 내용을 가져옵니다.
- **파라미터**:
- url: string (웹 페이지 URL)
- **수식**: HTTP GET 요청으로 내용 추출

### read_file
- **설명**: 파일 내용을 읽어옵니다.
- **파라미터**:
- filePath: string (읽을 파일 경로)
- **수식**: File.ReadAllText()

### write_file
- **설명**: 파일에 내용을 작성합니다.
- **파라미터**:
- filePath: string (작성할 파일 경로)
- content: string (작성할 내용)
- **수식**: File.WriteAllText()

### delete_file
- **설명**: 파일을 삭제합니다.
- **파라미터**:
- filePath: string (삭제할 파일 경로)
- **수식**: File.Delete()

### list_directory
- **설명**: 디렉토리 목록을 조회합니다.
- **파라미터**:
- path: string (조회할 디렉토리 경로, 선택사항. 생략 시 현재 디렉토리)
- **수식**: Directory.GetDirectories/GetFiles()

## Keywords
- system
- command
- file
- package
- install
- download
- search
- process
- directory
- cmd
- 명령어
- 파일
- 패키지
- 설치
- 다운로드
- 검색
- 프로세스
- 디렉토리
- 폴더
- winget
- 웹
- url
- read
- write
- delete
- list
- 정보

## Examples
- "현재 디렉토리 목록을 보여줘"
- "systeminfo 명령어를 실행해줘"
- "https://example.com/file.txt 파일을 다운로드해줘"
- "VSCode 를 설치해줘"
- "notepad.exe 프로세스를 찾아줘"
- "test.txt 파일에 'Hello World' 라고 써줘"
- "Google 에서 'C# 학습' 을 검색해줘"
- "https://www.google.com 페이지 내용을 가져와줘"
