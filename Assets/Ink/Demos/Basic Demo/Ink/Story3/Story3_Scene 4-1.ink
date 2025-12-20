
# show_panel:OnLock_Scam:fade
เวลาผ่านไปได้ไม่นาน ก็มีแจ้งเตือนขึ้นมาจาก PEA Official #VOICE:บรรยาย-15
# hide_panel:PhoneLock1

-> Line
== Line == 
# play_sound:Unlock
# show_panel:PhoneLock2
# show_panel:Chat_PEAOfficial

# hide_panel:PhoneLock2
# hide_panel:OnLock_Scam

//แชท
//# hide_panel:Chat_PEAOfficial

-> Scene4_3
== Scene4_3 == 
# show_panel:LivingRoom:fade
หลังจากการไฟฟ้าตอบคำถามของคุณ คุณก็ได้ข้อสรุปของคุณขึ้นมา  #VOICE:บรรยาย-17

คุณเลยเลือกที่จะ #VOICE:บรรยาย-18
    *[จ่ายค่ามิเตอร์ผ่านลิงก์]
        -> toScene4_2
    *[เดินทางไปติดต่อการไฟฟ้าโดยตรง]
	    -> Ending2GoodEnd
    *[ไม่เปลี่ยนมิเตอร์ จ่ายค่าไฟดังเดิม]
	    -> Ending3AlternativeEnd
//# hide_panel:Chat_PEAOfficial

== toScene4_2 ==
# load_ink:Story3_Scene 4-2
-> END

-> Ending1BadEnd 
== Ending1BadEnd ==
# hide_panel:Transfer1
# hide_panel:Transfer2
# show_panel:Bedroom:fade

สามวันผ่านไป คุณตื่นขึ้นมาพร้อมกับเสียงแจ้งเตือนของโทรศัพท์ #VOICE:จบ1-0

คุณคิดว่ามันเป็นเสียงแจ้งเตือนของเจ้าหน้าที่การ ไฟฟ้าที่จะมาเปลี่ยนมิเตอร์ให้คุณใหม่ #VOICE:จบ1-1

แต่พอคุณเปิดโทรศัพท์ขึ้นมาดู มันกลับเป็นรายการใช้จ่ายเงินในบัญชีคุณ #VOICE:จบ1-2

คุณตกใจมากกับเหตุการณ์ที่เกิดขึ้น #VOICE:จบ1-3


# show_panel:PEA:fade

คุณรีบแจ้งอายัดบัญชีและแจ้งความอย่างเร่งรีบ #VOICE:จบ1-4

โชคยังดีที่ตำรวจสามารถกู้เงินที่ถูกใช้ไปคืนมาได้บางส่วน #VOICE:จบ1-5

# show_panel:Ending1
# hide_panel:Bedroom
# hide_panel:PEA

-> END

//-> Ending2GoodEnd
== Ending2GoodEnd == 
# hide_panel:LivingRoom
# show_panel:PEA:fade

เช้าวันรุ่งขึ้น คุณได้เดินทางไปยังการไฟฟ้าส่วนภูมิภาค #VOICE:จบ2-0

คุณพูดคุยกับเจ้าหน้าที่เรื่องของการเปลี่ยนมิเตอร์ และการจ่ายค่าไฟ #VOICE:จบ2-1

เจ้าหน้าที่ได้ฟังเรื่องราวของคุณและกล่าวชื่นชมคุณ #VOICE:จบ2-2

อีกทั้งเจ้าหน้าที่ยังแนะนำคุณอีกด้วยว่า บัญชีของการไฟฟ้ามีเพียงบัญชีเดียวคือ @PEAThailand  #VOICE:จบ2-3

ซึ่งบัญชีแท้ต้องมีสัญลักษณ์โล่สีเขียวอยู่ข้างหน้าชื่อ #VOICE:จบ2-4

นอกจากจะคอยแจ้งเตือนข่าวสารและแจ้งค่าไฟแล้ว #VOICE:จบ2-5

คุณยังสามาถชำระค่าไฟในรูปออนไลน์ได้โดยไม่ต้องมาที่การไฟฟ้า #VOICE:จบ2-6

แต่เรื่องของมิเตอร์นั้น ต้องมาติดต่อกับการไฟฟ้าโดยตรงเท่านั้น #VOICE:จบ2-7


# hide_panel:PEA
# show_panel:Ending2
-> END
//-> Ending3AlternativeEnd
== Ending3AlternativeEnd ==
# show_panel:LivingRoom:fade

แล้วเวลาก็ล่วงเลยมาหนึ่งสัปดาห์ #VOICE:จบ3-0

คุณได้นำประสบการณ์ที่คุณเจอมาเล่าให้เพื่อนของคุณฟัง #VOICE:จบ3-1

เขาโล่งใจที่คุณไม่ได้ตกเป็นเหยื่อของมิจฉาชีพ #VOICE:จบ3-2

และบอกอีกด้วยว่าเขานั้นก็ยังใช้มิเตอร์จานหมุน แบบเก่าอยู่ #VOICE:จบ3-3

มันไม่ได้มีปัญหาอะไรเลย #VOICE:จบ3-4

ที่ค่าไฟขึ้นนั้น เป็นเพราะอากาศร้อน #VOICE:จบ3-5

ตู้เย็นและเครื่องปรับอากาศมักจะทำงานหนักใน ช่วงหน้าร้อนเป็นธรรมดา #VOICE:จบ3-6

ค่าไฟที่บ้านของเขาก็เพิ่มขึ้นเช่นกัน #VOICE:จบ3-7

คุณตัดสินใจเอาเรื่องราวที่เจอไปลองปรึกษาเพื่อน และแล้ว หัวข้อนี้ในการสนทนาก็เป็นเพียงเรื่อง ราวตลกขบขันที่แฝงไปด้วยสามัญสำนึกทั่วไปที่ คุณอาจจะลืมคิดถึงไปหรือเปล่านะ? #VOICE:จบ3-8
# hide_panel:LivingRoom
# show_panel:Ending3
-> END
-> PreEnding3AlternativeEnd
== PreEnding3AlternativeEnd ==
# show_panel:LivingRoom:fade
คุณสัมผัสได้ถึงบางสิ่งที่ผิดปกติ #VOICE:จบ3-10

รายชื่อบัญชีเป็นบัญชีส่วนบุคคล ไม่ใช่บัญชีทางการ #VOICE:จบ3-11

คุณจึงเลือกที่จะยกเลิกการโอน และเลิกติดต่อกับบัญชี Line นั้นไป #VOICE:จบ3-12

-> Ending3AlternativeEnd

-> END