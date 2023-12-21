using UnityEngine;
using System;
using TABLE;


public class DeviceProxy : MonoBehaviour{

    //private const int KEYCODE_SIZE = 8;

    public string MakeKeyString(long input){
        // 인풋 BitConverter 사용해서 bite 배열로 변경
        //주어진 64비트 정수를 바이트 배열로 변환하고,
        byte[] temp = BitConverter.GetBytes(input);

        if (temp.Length < KeyTable.KEY_TABLE_SIZE){
            // 예외 처리 또는 적절한 조치를 취해주세요.
            Debug.LogError("Input length is less than KeyTable.KEY_TABLE_SIZE");
            return string.Empty;
        }


        byte[] dest = new byte[KeyTable.KEY_TABLE_SIZE];
        int len = 0;

        Debug.Log("temp length: " + temp.Length);
        Debug.Log("dest length: " + len);

        Array.Clear(dest, 0, KeyTable.KEY_TABLE_SIZE);

        //해당 배열에서 0x00 바이트가 아닌 값들을 추출하여 키 문자열을 생성
        for (int i = 0; i < KeyTable.KEY_TABLE_SIZE; i++){
            if (temp[i] == 0x00)
                break;

            dest[i] = temp[i];
            len++;
        }

        if (len == 0)
            return string.Empty;

        // 마지막으로 키 문자열을 리버스
        Array.Reverse(dest, 0, len);

        return System.Text.Encoding.Default.GetString(dest, 0, len);
    }

    void InitKeyTable()
    {

    }


    //작동 예시
    private void Start(){
        long inputValue = 53199999999999999;
        string keyString = MakeKeyString(inputValue);

        Debug.Log("Generated Key String: " + keyString);
    }

}


