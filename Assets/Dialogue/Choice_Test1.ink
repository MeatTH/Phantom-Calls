//INCLUDE Test.ink

-> main

=== main ===
Test Choice
    + [c1]
        -> chosen("c1")
    + [c2] 
        -> next
        //-> INCLUDE Test.ink
        //INCLUDE Test.ink
        //->DONE
    + [c3] 
        Oh you choos c3.
- OK

what do you mean?
    + [i dont know]
     i dont know what do you talk about
    + [i know]
- its ok

-> END 
=== chosen(choice) ===
You chose {choice}
 *[แตะเพื่อไปต่อ] Choice way2
 **[แตะเพื่อไปต่อ] Choice way3
    ***[แตะเพื่อไปต่อ] Choice way4

-> END

=== next ===
//INCLUDE Test.ink


->END



        