# Skill: Translator

## Description
한국어와 일본어 간의 번역을 수행하는 스킬입니다.
게임 Localization 작업이나 일본 게임 정보 이해를 돕습니다.

## Functions

### translate_ko_to_ja
- **설명**: 한국어 텍스트를 일본어로 번역합니다.
- **파라미터**:
- text: string (번역할 한국어 텍스트)
- **수식**: 한국어 → 일본어 번역

### translate_ja_to_ko
- **설명**: 일본어 텍스트를 한국어로 번역합니다.
- **파라미터**:
- text: string (번역할 일본어 텍스트)
- **수식**: 일본어 → 한국어 번역

### translate_game_term
- **설명**: 게임 용어를 번역합니다.
- **파라미터**:
- term: string (게임 용어)
- sourceLang: string (소스 언어, 'ko' 또는 'ja')
- targetLang: string (타겟 언어, 'ko' 또는 'ja')
- **수식**: 게임 용어 번역

## Examples
- "안녕하세요를 일본어로 번역해줘"
- "こんにちは를 한국어로 번역해줘"
- "공격력이라는 게임 용어를 일본어로 어떻게 써?"
- "ダメージ를 번역해줘"
