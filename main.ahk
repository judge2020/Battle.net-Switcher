email = %1%
passwrd = %2%

Checkd()
{
    WinWait Blizzard App Login, ,3
    WinMove 0, 0
    WinActivate
    
    IfWinNotActive
    {
        Checkd()
    }
    if ErrorLevel
    {
        MsgBox unable to find Battle.net login window. Please record trying to login and report it at j20.pw/bnethelp
        exit
    }
}
Checkd()
Sleep 2000
Checkd()
WinActivate
Checkd()
ClickFromTopRight(42,200)
Send ^a
Send {BS}
send %email%
sleep 20
Sleep 100
ClickFromTopRight(42,250)
Sleep 20
ClickFromTopRight(42,250)
Send ^a
Send {BS}
send %passwrd%
Send {Enter}
exit


ClickFromTopRight(_X,_Y){

CoordMode, mouse, Relative

WinGetActiveStats, Title, width, height, x,y

_X := width - _X

Click %_X%, %_Y%

}

