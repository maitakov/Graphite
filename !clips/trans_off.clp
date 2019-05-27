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


(defrule determine-have-damage-inf ""
   (not (line-damaged-inf ?))
   (not (repair ?))
   =>
   (if (yes-or-no-p "Есть информация о явном повреждении оборудования  (yes/no)? ") 
       then 
       (assert (repair "Следует вывести линию в ремонт."))
       else 
       (assert (line-damaged-inf havent))))





(defrule determine-deadend-or-transit ""
   (line-damaged-inf havent)
   (not (deadend-or-transit ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "Произошло аварийное отключение тупиковой или транзитнйо линии (deadend/transit)? " 
			deadend transit))
       	
       	(if (eq ?response deadend)
		then
		  (if (yes-or-no-p "Опробуйте линию напряжением.Линия работает стабильно (yes/no)? ") 
       		then 
       			(assert (line-damaged havent))
       		else 
       			(assert (repair "Следует вывести линию в ремонт.")))

		(assert (deadend-or-transit deadend))


	else (if (eq ?response transit)
                then
		 (if (yes-or-no-p "Опробуйте линию напряжением.Линия работает стабильно (yes/no)? ") 
       		then 
       			(assert (line-damaged havent))
       		else 
       			(assert (repair "Следует вывести линию в ремонт.")))

		(assert (deadend-or-transit transit)))))













(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "Рекомендации по устранению аварийной ситуации:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
