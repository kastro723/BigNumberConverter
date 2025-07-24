# BigNumberConverter
        [패치 노트]
        
                v1.0.1 -24.02.23 
                       - null 및 빈 문자열 오류 수정


                v1.0.0 - 24.02.22
                       - Big Number Converter 기능 구현

-------------------------------------------------------------------------------------
<img width="400" height="668" alt="image" src="https://github.com/user-attachments/assets/bf83b061-a01f-4618-a730-f0d5ba6ff236" />


        [기능설명]
        
                큰 숫자 변환기 & 계산기


        [공통 사항]
                
                빈 문자열 = 0 반환

                음수로 내려가면 0반환
                
                숫자/소숫점(.)/대문자를 제외한 문자 입력시 Invalid Input 반환


        
        <Big Number Converter> - 숫자 재화 단위 변환기(A~Z,AA,AAA...ZZZ)
    
                TextArea에 숫자 입력 후, Convert 버튼 클릭


                        
        <Back to Big Number Converter> - 숫자+문자(A~Z, AA, AAA...)
                                                -> 리얼넘버(123412..) 변환기

                TextArea에 숫자 혹은 숫자+문자 입력 후, 
                        Convert Back To Number 버튼 클릭

                        
                
                <Big Number Calculator> -  Big Number 사칙연산 계산기

                       Number A, Number B에 숫자를 넣고 +,-,*,/ 버튼을 통해 사칙연산
                        


        [**지원 형식**]
        
                문자와 문자 
                
                        ex) 1A + 1A = 2A
                        
                숫자와 문자
                
                        ex) 123 + 1A = 1.12A
                        
                숫자와 숫자
                
                        ex) 123 + 456 = 579
                                
