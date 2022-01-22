; JEZYKI ASEMBLEROWE - PROJEKT
; Data: 20.01.2022, wersja: 1.0
; Autor: Jerzy Balcer, Informatyka Katowice, rok 3 sem. 5, gr. 1
; Temat: Korekcja kolorow bitmapy przez balans kanalow RGB
; Opis: Algorytm mnozy oryginalne wartoci RGB przez wspolczynnik bedacy procentem oryginalnej wartosci zmieniajac jednoczesnie kolor piksela

.code
Correct proc

;;;;;;;;;;;;;;;;;;;;;;;; 3 PARAMS MASK METHOD - DISABLED BY DEFAULT
; SETUP FILTER MASK [BLUE, GREEN, RED, BLUE] 
;MOVD EAX, XMM3 ;blue parameter
;PINSRD XMM4, EAX, 0
;PINSRD XMM4, EAX, 3 ;blue is repeated at the end because input array contains BGRB values

;MOVD EAX, XMM2 ;green parameter
;PINSRD XMM4, EAX, 1

;MOVD EAX, XMM1 ;red parameter
;PINSRD XMM4, EAX, 2
;;;;;;;;;;;;;;;;;;;;;;;;

; LOAD MASK TO XMM REGISTER (MASK ARRAY METHOD)
MOVUPS XMM4, XMMWORD PTR [RDX]

; LOAD RGB VALUES TO XMM REGISTER
MOVD XMM2, DWORD PTR [RCX] ;move 4 RGB bytes to xmm2

; CONVERT 8 BIT INT VALUES TO 16 BIT
VPMOVZXBD XMM2, XMM2

; CONVERT 16 BIT INTS TO 32 BIT FLOATS (FLOAT IS NEEDED BECAUSE COLOR IS MULTIPLIED BY PERCENT)
CVTDQ2PS XMM2, XMM2

; MULTIPLY RGB VALUES BY MASK
MULPS XMM4, XMM2

; CONVERT 32 BIT FLOATS TO 32 BIT INTS	
CVTPS2DQ XMM1, XMM4

; CONVERT 32 BIT INTS TO 16 BIT INTS
PACKUSDW XMM1, XMM1

; CONVERT 16 BIT INTS TO 8 BIT INTS IN ORDER TO STORE IN A BYTE ARRAY
PACKUSWB XMM1, XMM1

; RETURN CORRECTED RGB VALUES ;
MOVD RSI, XMM1 ;move RGB Values to rsi
MOV [RCX], RSI ;move RGB Values to output array address

ret
Correct endp
end
