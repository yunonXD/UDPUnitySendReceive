using UnityEngine;

public class HangulProcessor : MonoBehaviour
{
    private static string 초성Tbl = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
    private static string 중성Tbl = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
    private static string 종성Tbl = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
    private static ushort UniCode한글Base = 0xAC00;
    private static ushort UniCode한글Last = 0xD79F;
    private static string m초성;
    private static string m중성;
    private static string m종성;

public static string 자소합치기(string s초성, string s중성, string s종성)
{
    int i초성위치, i중성위치, i종성위치;
    int iUniCode;
    i초성위치 = 초성Tbl.IndexOf(s초성);   // 초성 위치
    i중성위치 = 중성Tbl.IndexOf(s중성);   // 중성 위치
    i종성위치 = 종성Tbl.IndexOf(s종성);   // 종성 위치

    if (i초성위치 < 0 || i중성위치 < 0 || i종성위치 < 0)
    {
        // 처리할 수 없는 값이 들어왔을 때
        return "";
    }

    // 앞서 만들어 낸 계산식 수정
    iUniCode = UniCode한글Base + i초성위치 * 21 * 28 + i중성위치 * 28 + i종성위치;

    if (iUniCode < UniCode한글Base || iUniCode > UniCode한글Last)
    {
        // 처리할 수 없는 값이 계산되었을 때
        return "";
    }

    // 코드값을 문자로 변환
    char temp = (char)iUniCode;
    return temp.ToString();
}

public void 자소나누기(char c한글자)
{
    int i초성Idx, i중성Idx, i종성Idx; // 초성, 중성, 종성의 인덱스
    ushort uTempCode = c한글자;

    // 캐릭터가 한글이 아닐 경우 처리
    if ((uTempCode < UniCode한글Base) || (uTempCode > UniCode한글Last))
    {
        m초성 = ""; m중성 = ""; m종성 = "";
    }
    else
    {
        // iUniCode에 한글코드에 대한 유니코드 위치를 담고 이를 이용해 인덱스 계산.
        int iUniCode = uTempCode - UniCode한글Base;
        i초성Idx = iUniCode / (21 * 28);
        iUniCode = iUniCode % (21 * 28);
        i중성Idx = iUniCode / 28;
        iUniCode = iUniCode % 28;
        i종성Idx = iUniCode;
        m초성 = 초성Tbl.Substring(i초성Idx, 1);
        m중성 = 중성Tbl.Substring(i중성Idx, 1);
        m종성 = (i종성Idx > 0) ? 종성Tbl.Substring(i종성Idx - 1, 1) : "";
    }
}

   
    private void Start()
    {
        char cWork = '끍';
        자소나누기(cWork);
        Debug.Log("초성: " + m초성);
        Debug.Log("중성: " + m중성);
        Debug.Log("종성: " + m종성);

        string composedResult = 자소합치기(m초성, m중성, m종성);
        Debug.Log("자소합치기 결과: " + composedResult);
    }
}
