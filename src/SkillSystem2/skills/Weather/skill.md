# Skill: Weather

## Description
가상 날씨 정보를 제공하는 스킬입니다.
현재 날씨, 주말 예보, 지역별 날씨가상 쿼리를 처리합니다.

## Functions

### get_current_weather
- **설명**: 현재 지역의 날씨 정보를 조회합니다.
- **파라미터**:
- location: string (지역 이름, 예: 서울, 부산, 제주)
- **수식**: 가상의 날씨 데이터 반환

### get_weekend_forecast
- **설명**: 주말 (토요일, 일요일) 날씨 예보를 조회합니다.
- **파라미터**:
- location: string (지역 이름)
- **수식**: 가상의 주말 예보 데이터 반환

### compare_weather
- **설명**: 두 지역의 날씨를 비교합니다.
- **파라미터**:
- location1: string (첫 번째 지역)
- location2: string (두 번째 지역)
- **수식**: 두 지역 날씨 비교 결과 반환

## Keywords
- weather
- forecast
- 날씨
- 예보
- 비
- 맑음
- 구름
- 온도
- 기온
- 강수
- 주말
- 비교

## Examples
- "서울 현재 날씨가 어때?"
- "제주도 주말 날씨 예보 알려줘"
- "부산과 도쿄 날씨 비교해줘"
- "오늘 비 올까?"
