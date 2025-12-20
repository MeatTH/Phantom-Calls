
//# stop_bgm
//# hide_panel:BedRoom
->Phone
== Phone ==
“ไม้” คือลูกชายของคุณเขาไปทำงานที่กรุงเทพฯ ได้หลายปีแล้ว #VOICE:บรรยาย-10 # hide_panel:BedRoom # show_panel:PhoneLock # show_panel:OnLock:fade



//# play_sound:Line_message
ทุกวันก่อนเข้าทำงาน เขามักจะส่งข้อความมาหา คุณเพื่อถามถึงความเป็นอยู่   #VOICE:บรรยาย-11
//# show_panel:OnLock

-> inPhone

-->END

== inPhone ==
# play_sound:Unlock # show_panel:Chat2-1 # hide_panel:PhoneLock # hide_panel:OnLock:fade
//แสดงแชท
//# hide_panel:Chat2-1

-> Line

--> END

== Line ==  
# show_panel:LivingRoom

# load_ink:Story1_Scene 3-1

- -> END

//  สวัสดีครับแม่ ผมถึงที่ทำงานแล้วนะครับ #speaker:ลูกชาย #layout:left
//  เช้านี้ฝนตกไหมครับ? ที่กรุงเทพฝนตกหนักเลย #speaker:ลูกชาย #layout:left
//  ถนนก็ติดมาก ๆ กว่าจะมาถึงทีทำงานก็ลุ้นแทบแย่ #speaker:ลูกชาย #layout:left
 
// ไม่ตกเลยลูก อากาศกำลังดีเลย #speaker:แม่ #layout:left
// ที่นี่ก็ตกเหมือนกันลูก #speaker:แม่ #layout:left
// แล้วลูกกินอะไรตอนเช้าหรือยัง? #speaker:แม่ #layout:left

// กินแล้วครับแม่ วันนี้ผมกินโจ๊กหมูใส่ไข่ครับ อร่อยมากเลย #speaker:ลูกชาย #layout:left
// อ้อ เกือบลืมบอกไปครับแม่ #speaker:ลูกชาย #layout:left
// พัสดุที่จะส่งถึงบ้านแม่อันต่อไปไม่ใช่อาหารเสริมของแม่นะมันเป็นของที่ผมสั่งของเอาไว้ #speaker:ลูกชาย #layout:left
// วันหยุดยาวนี้ผมจะกลับบ้านไปหาแม่ ฝากรับของแทนผมด้วยนะครับ #speaker:ลูกชาย #layout:left

// จ้า ดูแลสุขภาพตัวเองด้วยนะ แม่เป็นห่วง  #speaker:แม่ #layout:left

// ถึงงานจะหนักไปบ้าง แต่เพื่อแม่แล้ว ผมสู้ครับ #speaker:ลูกชาย #layout:left

// อย่าหักโหมมากนักนะลูก พักผ่อนให้พอ #speaker:แม่ #layout:left 
// งานน่ะทำไปเรื่อย ๆ ก็ได้ ไม่ต้องรีบขนาดนั้น #speaker:แม่ #layout:left

// ครับแม่ แค่นี้นะครับ เดี๋ยวผมขอตัวไปทำงานก่อน #speaker:ลูกชาย #layout:left

// ตั้งใจทำงาน แม่รักลูกนะ #speaker:แม่ #layout:left

// รักแม่เหมือนกันครับ #speaker:ลูกชาย #layout:left

