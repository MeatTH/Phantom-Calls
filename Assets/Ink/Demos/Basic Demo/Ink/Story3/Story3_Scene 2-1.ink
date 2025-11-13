# hide_panel:LivingRoom
# show_panel:PhoneLock
# play_bgm:

->Phone
== Phone ==
# show_panel:PhoneLock
# show_panel:OnLock

หน้าจอโทรศัพท์ปรากฏข้อความแจ้งเตือนไลน์จากการไฟฟ้าส่วนภูมิภาค

เวลานี้มาถึงแล้ว 

ค่าไฟเป็นรายจ่ายเล็กๆ ที่คุณไม่สามารถหนีมันพ้นได้

-> inPhone
== inPhone ==
# play_sound:Unlock
# show_panel:Chat_PEAThailand
แสดงแชท
# hide_panel:Chat_PEAThailand
# hide_panel:PhoneLock
# hide_panel:OnLock
//# hide_panel:Chat_PEAThailand

-> Test
== Test ==
# show_panel:LivingRoom
ค่าไฟเดือนนี้ก็แพงขึ้นอีกแล้วหรอ?

เมื่อสิ้นปีจ่ายค่าไฟแค่ 2,000 เองนะ

# hide_panel:LivingRoom

-> Line
== Line == 
//# hide_panel:Chat_PEAThailand

# show_panel:PhoneLock
# show_panel:Chat_PEAOfficial
แสดงแชท
# hide_panel:PhoneLock
# hide_panel:Chat_PEAOfficial

-> NextScene
== NextScene ==

# load_ink:Story3_Scene 3-1

- -> END


