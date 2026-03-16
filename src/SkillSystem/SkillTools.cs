namespace SkillSystem;

public class SkillTools
{
    public double CalculateDps(int damage, double attacksPerSecond)
    {
        return damage * attacksPerSecond;
    }

    public double CalculateCritDamage(double baseDamage, double critMultiplier)
    {
        return baseDamage * critMultiplier;
    }

    public double CalculateExpectedDps(double dps, double critChance, double critMultiplier)
    {
        return dps * (1 + (critChance * (critMultiplier - 1)));
    }

    public string GetCurrentWeather(string location)
    {
        var weathers = new Dictionary<string, string>
        {
            { "서울", "맑음, 23°C, 미세먼지 보통" },
            { "부산", "구름 조금, 25°C, 습도 60%" },
            { "제주", "비, 21°C, 강수확률 80%" },
            { "도쿄", "맑음, 26°C, 습도 50%" },
            { "오사카", "흐림, 24°C, 저녁에 비 예상" }
        };

        if (weathers.TryGetValue(location, out var weather))
        {
            return $"{location} 현재 날씨: {weather}";
        }

        return $"{location}의 날씨 정보를 찾을 수 없습니다. (서울, 부산, 제주, 도쿄, 오사카 중 하나를 입력하세요)";
    }

    public string GetWeekendForecast(string location)
    {
        var forecasts = new Dictionary<string, string>
        {
            { "서울", "토요일: 맑음, 25°C / 일요일: 구름 조금, 27°C" },
            { "부산", "토요일:晴れ, 28°C / 일요일: 흐림, 26°C" },
            { "제주", "토요일: 비, 22°C / 일요일: 흐림, 23°C" },
            { "도쿄", "토요일: 晴れ, 29°C / 일요일: 曇り, 27°C" },
            { "오사카", "토요일: 曇り, 26°C / 일요일: 雨, 24°C" }
        };

        if (forecasts.TryGetValue(location, out var forecast))
        {
            return $"{location} 주말 예보: {forecast}";
        }

        return $"{location}의 주말 예보를 찾을 수 없습니다.";
    }

    public string CompareWeather(string location1, string location2)
    {
        var weather1 = GetCurrentWeather(location1);
        var weather2 = GetCurrentWeather(location2);
        
        return $"""
            날씨 비교 결과:
            - {weather1}
            - {weather2}
            """;
    }

    public string TranslateKoToJa(string text)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "안녕하세요", "こんにちは" },
            { "감사합니다", "ありがとうございます" },
            { "공격력", "攻撃力" },
            { "방어력", "防御力" },
            { "생명력", "生命力" },
            { "마나", "マナ" },
            { "스킬", "スキル" },
            { "아이템", "アイテム" }
        };

        if (dictionary.TryGetValue(text, out var translation))
        {
            return $"{text} → {translation}";
        }

        return $"'{text}'에 대한 번역 사전이 없습니다. (안녕하세요, 감사합니다, 공격력, 방어력, 생명력, 마나, 스킬, 아이템 중 하나)";
    }

    public string TranslateJaToKo(string text)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "こんにちは", "안녕하세요" },
            { "ありがとうございます", "감사합니다" },
            { "攻撃力", "공격력" },
            { "防御力", "방어력" },
            { "生命力", "생명력" },
            { "マナ", "마나" },
            { "スキル", "스킬" },
            { "アイテム", "아이템" },
            { "ダメージ", "데미지" }
        };

        if (dictionary.TryGetValue(text, out var translation))
        {
            return $"{text} → {translation}";
        }

        return $"'{text}'에 대한 번역 사전이 없습니다. (こんにちは, ありがとうございます, 攻撃力，防御力，生命力，マナ，スキル，アイテム，ダメージ 중 하나)";
    }

    public string TranslateGameTerm(string term, string sourceLang, string targetLang)
    {
        if (sourceLang == "ko" && targetLang == "ja")
        {
            return TranslateKoToJa(term);
        }
        else if (sourceLang == "ja" && targetLang == "ko")
        {
            return TranslateJaToKo(term);
        }

        return "지원하지 않는 언어 쌍입니다. (ko, ja 만 지원)";
    }
}
