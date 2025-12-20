จนเวลาได้ล่วงเลยมาถึงช่วงบ่าย #VOICE:บรรยาย-12 # show_panel:LivingRoom

เสียงโทรเข้าก็ได้ดังทำลายบรรยากาศที่เงียบสงบ #VOICE:บรรยาย-13 # show_panel:LivingRoom

คุณนึกขึ้นได้ว่ามีของจะมาส่งจึงหยิบมือถือขึ้นมา เพื่อที่จะรับโทรศัพท์ #VOICE:บรรยาย-14 # show_panel:LivingRoom


-> show_callin

== show_callin ==
//# play_bgm:ringtone
# show_panel:PhoneCallIn 
คุณได้กดวางสายโทรศัพท์  
# hide_panel:PhoneCallIn  
//# stop_bgm
-> call
-> END
== call ==
//# stop_bgm
//# show_panel:PhoneCalling # hide_panel:LivingRoom # hide_panel:PhoneCallIn  
"สวัสดีครับ มีพัสดุมาส่งครับ กรุณาออกมารับด้วยครับ" #speaker:คนส่งของ #VOICE:พากย์-0 # show_panel:PhoneCalling # hide_panel:LivingRoom # hide_panel:PhoneCallIn  
 * [ออกไปรับพัสดุ] "ค่ะ กำลังไปรับค่ะ" # show_panel:Home # play_sound:Walk #speaker:คุณ #VOICE:พากย์-1 # hide_panel:PhoneCalling 

คุณเดินไปที่ประตูหน้าบ้านและเปิดประตู เพื่อที่จะเซ็นชื่อรับของ # play_sound:OpenDoor #VOICE:บรรยาย-15 # show_panel:Home

 -> livingroom
 
 == livingroom ==
 "จัดเก็บเงินปลายทาง 1,500 บาทครับ" #VOICE:พากย์-2 # show_panel:Home  #speaker:คนส่งของ

    * [เก็บเงินปลายทางหรือ?] "มีเก็บเงินปลายทางด้วยเหรอคะ" #speaker:คุณ #VOICE:พากย์-3 # show_panel:Home

        "ใช่ครับ" #speaker:คนส่งของ #VOICE:พากย์-4 # show_panel:Home
        
//# hide_panel:LivingRoom

-> choice

== choice==
 *[สอบถามข้อมูลของพัสดุ] -> Scene3_3
 *[โทรถามลูกชาย]  -> Scene3_4

== Scene3_3 ==
 "ขอทราบรายละเอียดพัสดุได้ไหมคะ?" #speaker:คุณ #VOICE:พากย์-5 # show_panel:Home
 
 "ผู้รับชื่อคุณไม้" #speaker:คนส่งของ #VOICE:พากย์-6 # show_panel:Home
 
 "ที่อยู่จัดส่ง บ้านเลขที่ 121 หมู่ 3 ต.บ้านหนองกึ่ม" #speaker:คนส่งของ #VOICE:พากย์-7 # show_panel:Home
 
 "ข้อมูลถูกต้องไหมครับ" #speaker:คนส่งของ #VOICE:พากย์-8 # show_panel:Home
    *[ข้อมูลถูกต้อง]  "ถูกค่ะ" #speaker:คุณ #VOICE:พากย์-9 # show_panel:Home

-> Scene3_5
 
== Scene3_4 ==
"สักครู่นะคะ" #speaker:คุณ #VOICE:พากย์-33

คุณโทรหาลูกชายอยู่นาน แต่ก็ไม่มีการตอบรับใดๆ #VOICE:บรรยาย-16

แต่คุณก็นึกขึ้นได้ ลูกชายบอกกับคุณว่าเขาสั่งของเอาไว้ #VOICE:บรรยาย-17
-> Scene3_5


== Scene3_5 ==
//"ถูกค่ะ"

"คือว่า... รีบหน่อยได้ไหมครับ" #speaker:คนส่งของ #VOICE:พากย์-10 # show_panel:Home # hide_panel:Box

"พอดีผมต้องไปส่งของที่อื่นต่อ" #speaker:คนส่งของ #VOICE:พากย์-11 # show_panel:Home # hide_panel:Box

  *[ขอโทษขนส่ง]"อะ... ขอโทษที่ทำให้เสียเวลาค่ะ" #speaker:คุณ #VOICE:พากย์-12 # show_panel:Home # hide_panel:Box
  
//     **[จ่ายเงิน]
// // # hide_panel:LivingRoom    
// // # show_panel:Box
คุณจ่ายเงินให้กับพนักงานส่งของ 1,500 บาท #VOICE:บรรยาย-18 # show_panel:Home # hide_panel:Box # hide_panel:LivingRoom
จากนั้นก็รับพัสดุของลูกชายมาวางไว้ในห้องนั่งเล่น # play_sound:CloseDoor #VOICE:บรรยาย-19 # hide_panel:Home    # show_panel:Box # hide_panel:LivingRoom
คุณพิมพ์บอกลูกชายว่ารับพัสดุมา พร้อมกับจ่ายเงินให้แล้ว #VOICE:บรรยาย-20

ช่วงเย็นในเวลาเลิกงาน #VOICE:บรรยาย-30 # hide_panel:Box # show_panel:LivingRoom

ลูกชายโทรมาหาคุณเพื่อขอโทษที่ไม่ได้รับสาย เนื่องจากงานยุ่งมาก นอกจากนี้เขายังขอโทษ เรื่องที่ต้องให้คุณจ่ายเงินแทน #VOICE:บรรยาย-21 # hide_panel:Box # show_panel:LivingRoom

เพราะเขาคุ้นเคยกับการสั่งของให้คุณเสมอ และคุณก็มักจะเป็นคนจ่ายเงินปลายทางเองทุกครั้ง #VOICE:บรรยาย-22 # hide_panel:Box # show_panel:LivingRoom
# load_ink:Story1_Scene 4-1

-> END








