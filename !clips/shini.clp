;;;********************************************************************
;;;  Пример экспертной системы на языке CLIPS, позволяющей  диагностиро-вать 
;;;    некоторые неисправности автомобиля  и предоставлять пользователю 
;;;    рекомендации по их устранению  
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






(defrule determine-what-with-bus ""
   (not (what-with-bus ?))
   (not (why-bus-off ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "Сборные шины отключились(opt1), исчезло напряжение вследствие отказа защиты (opt2), шины обеточились (opt3) ? (opt1/opt2/opt3)? " 
			opt1 opt2 opt3))
       	
       	(if (eq ?response opt1)
		then
		  (assert (what-with-bus off))
		  

		else (if (eq ?response opt2)
               		then
			 (assert (what-with-bus voltage-missing))
			

		else (if (eq ?response opt3)
        	        then
			 (assert (what-with-bus bus-de-energized))))))




(defrule determine-def-or-UROV ""
   (what-with-bus off)
   (not (def-or-UROV ?))
   (not (repair ?))
   =>
   	(bind ?response 
	(ask-question "Шины отключились действием деференциальнйо зищиты шин или УРОВ (def/UROV)? "
             def urov))
   		(if (eq ?response def) 
       then 
	(assert (def-or-UROV def))  ; 
       else (if (eq ?response zagr)
                then
		 (assert (def-or-UROV UROV)))))


(defrule determine-bus-breaker-status ""
   (def-or-UROV def)
   (not (bus-breaker-status ?))
   (not (repair ?))
   =>
   	(if (yes-or-no-p "Произошел ли отказ шиносоединительного выключателя (yes/no)? ") 
       	then 
       	(assert (bus-breaker-status broken))
	   
       	else 
      	(assert (bus-breaker-status normal))))


(defrule repair-with-bus-breaker ""
   (def-or-UROV def)
   (bus-breaker-status broken)
   (not (bus-breaker-status ?))
   (not (repair ?))
   =>
   	(if (yes-or-no-p "Произошел ли отказ шиносоединительного выключателя (yes/no)? ") 
       	then 
       	(assert (bus-breaker-status broken))
	   
       	else 
      	(assert (bus-breaker-status normal))))


(defrule determine-def-repair ""
   (def-or-UROV def)
   (bus-breaker-status normal)
   (not (repair ?))
   =>
   (if (yes-or-no-p "Следует убедиться, что в распределительном устройсте персонал отсутсвует. Персонал присутсвует? (yes/no)? ") 
       then 
       (if (yes-or-no-p "Выведите персонал в безопасное место.Возможно ли подать напряжение на шины действием устройств АПВ или АВР шин?  (yes/no)? ")
           then (assert (repair "Подайте напряжение на шины действием устройств АПВ или АВР шин."))
           else (assert (repair "Подайте напряжение на шины вручную, по любой транзитной линии или от любого присоединения имеющего напряжение.")))
       else 
       (if (yes-or-no-p "Возможно ли подать напряжение на шины действием устройств АПВ или АВР шин?  (yes/no)? ")
           then (assert (repair "Подайте напряжение на шины действием устройств АПВ или АВР шин."))
           else (assert (repair "Подайте напряжение на шины вручную, по любой транзитной линии или от любого присоединения имеющего напряжение.")))))



(defrule determine-manually-automatics ""
   (def-or-UROV UROV)
   (bus-breaker-status normal)
   (not (repair ?))
   =>
   (if (yes-or-no-p "Следует убедиться, что в распределительном устройсте персонал отсутсвует. Персонал присутсвует? (yes/no)? ") 
       then 
       (if (yes-or-no-p "Выведите персонал в безопасное место.Возможно ли подать напряжение на шины действием устройств АПВ или АВР шин?  (yes/no)? ")
           then (assert (repair "Подайте напряжение на шины действием устройств АПВ или АВР шин."))
           else (assert (repair "Подайте напряжение на шины вручную, по любой транзитной линии или от любого присоединения имеющего напряжение.")))
       else 
       (if (yes-or-no-p "Возможно ли подать напряжение на шины действием устройств АПВ или АВР шин?  (yes/no)? ")
           then (assert (repair "Подайте напряжение на шины действием устройств АПВ или АВР шин."))
           else (assert (repair "Подайте напряжение на шины вручную, по любой транзитной линии или от любого присоединения имеющего напряжение.")))))










(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "Рекомендации по устранению аварийной ситуации:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
