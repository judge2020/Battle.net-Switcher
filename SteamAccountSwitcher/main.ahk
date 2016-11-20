email = %1%
passwrd = %2%

Checkd()
{
    WinWait Battle.net Login, ,3
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
Click 322, 162
Send ^a
Send {BS}
send %email%
sleep 20
Sleep 100
Click 322, 215
Sleep 20
Click 322, 215
Send ^a
Send {BS}
send %passwrd%
Send {Enter}
exit