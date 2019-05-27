;;;********************************************************************
;;;  
;;;    
;;;     
;;;
;;;                                       CLIPS Version 6.3 Example
;;;
;;;********************************************************************
;;;     Вспомогательные функции
;;; ********************************************************************
;;; Функция ask-question задает пользователю вопрос, полученный
;;;  в переменной ?question, и получает от пользователя ответ,
;;;  принадлежащий списку допустимых ответов, заданному в $?allowed-values 

(deffunction ask-question (?question $?allowed-values)
   (printout t ?question)
   (bind ?answer (read))
   (if (lexemep ?answer) 
       then (bind ?answer (lowcase ?answer)))
   (while (not (member$ ?answer ?allowed-values)) do
      (printout t ?question)
      (bind ?answer (read))
      (if (lexemep ?answer) 
          then (bind ?answer (lowcase ?answer))))
   ?answer)

;;;  Функция yes-or-no-p задает пользователю вопрос, полученный
;;;  в переменной ?question, и получает от пользователя ответ yes(у)или
;;;  no(n). В случае положительного ответа функция возвращает значение TRUE,
;;;  иначе - FALSE

(deffunction yes-or-no-p (?question)
   (bind ?response (ask-question ?question yes no y n))
   (if (or (eq ?response yes) (eq ?response y))
       then TRUE 
       else FALSE))


;;;******************************************************************
;;;     Диагностические правила
;;;*****************************************************************

(defrule determine-breaker-status ""
   (not (breaker-status ?))
   (not (repair ?))
   =>
   (if (yes-or-no-p "Несимметричный режим вызван неполнофазным отключением или включением выключателя линии (yes/no)? ") 
       	then 
       	(if (yes-or-no-p "Попробуйте отключить выключатель. Выключатель отключился? (yes/no)? ")
           then (assert (breaker-status normal))
           else (assert (breaker-status broken)))
	   
       	else 
      	(assert (breaker-status normal))))



(defrule if-the-breaker-is-broken ""
   (breaker-status broken)
   (not (local-control-button ?))
   (not (repair ?))   
   =>
   (if (yes-or-no-p "Примите экстренные меры по разгрузке генераторов и отключите выключатель кнопкой местного управления. Выключатель отключился(yes/no)?  ")
       then	
	(assert (local-control-button turned-off))            	
	             
       
       else 
	(assert (repair "Выключите линию с неисправным выключателем на опробованную напряжением обходную систему шин, затем включите обходной выключатель и с нарушением блокировки отключите линейные и шинные разъединители поврежденного выключателя.")))) 
              

(defrule determine-deadend-or-transit ""
   (breaker-status normal)
   (not (deadend-or-transit ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "Неполнофазный режим возник на тупиковой или транзитной линии (deadend/transit)? " 
			deadend transit))
       	
       	(if (eq ?response deadend)
		then
		  (assert (deadend-or-transit deadend))

	else (if (eq ?response transit)
                then
		 (assert (deadend-or-transit transit)))))


(defrule determine-deadend-with-branches ""
   (deadend-or-transit deadend)
   (not (deadend-with-branches ?))
   (not (repair ?))
   =>
   (if (yes-or-no-p "Эта тупиковая линия с ответвлениями (yes/no)? ") 
       	then 	
	(assert (deadend-with-branches with-branches)) 

	else
	(assert (repair "Следует заземлить нейтраль трансформатора этой линии, путем включения заземляющего ножа"))))

(defrule determine-transit-with-branches ""
   (deadend-or-transit transit)
   (not (transit-with-branches ?))	
   (not (repair ?))
   =>
   (if (yes-or-no-p "Эта транзитная линия с ответвлениями (yes/no)? ") 
       	then 	
	(assert (transit-with-branches with-branches)) 

	else
	(assert (repair "Следует рагрузить и отключить линию, далее вывести ее в ремонт"))))




(defrule do-on-deadend-with-branches ""
   (deadend-with-branches with-branches)
   (not (repair ?))
   =>
 (bind ?response	
   (ask-question "Следует  выявить на каком оборудовании произошел разрыв фаз путем проверки следующих параметров. Величина несеметрии на гнераторах(option1). 
			Токовая нагрузка трансформаторах, также уровни и симметрию напряжений низкой стороны 
			трансформаторов (option2).Какие параметры находяся не в норме? (option1/option2)? " 
			option1 option2))
       	
       	(if (eq ?response option1)
		then
		  (assert (repair "Следует разгрузить генераторы"))

	else (if (eq ?response option2)
                then
		 (assert (repair "Следует снизить нагрузку трансформаторов путем включения заземления нейтралей других трансформаторов")))))



(defrule do-on-transit-with-branches ""
   (transit-with-branches with-branches)
   (not (repair ?))
   =>
 (bind ?response	
   (ask-question "Следует перевисти эту линию в тупиковый режим и далее выявить на каком оборудовании произошел разрыв фаз путем проверки следующих параметров. Величина несеметрии на гнераторах(option1). Токовая нагрузка трансформаторах, также
		    уровни и симметрию напряжений низкой стороны трансформаторов (option2).Какие параметры находяся не в норме? (option1/option2)? " 
			option1 option2))
       	
       	(if (eq ?response option1)
		then
		  (assert (repair "Следует разгрузить генераторы"))

	else (if (eq ?response option2)
                then
		 (assert (repair "Следует снизить нагрузку трансформаторов, путем включения заземления нейтралей других трансформаторов")))))










(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "Рекомендации по устранению аварийной ситуации:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
