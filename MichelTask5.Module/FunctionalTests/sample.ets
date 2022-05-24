;#DropDB MichelTask5EasyTest

#Application MichelTask5Win
#Application MichelTask5Web
#IfDef MichelTask5Win

*Action About...
 
*Action OK

#EndIf

#IfDef MichelTask5Web
!Action Navigation(My Details)
#EndIf
