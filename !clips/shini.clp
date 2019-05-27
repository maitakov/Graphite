;;;********************************************************************
;;;  ������ ���������� ������� �� ����� CLIPS, �����������  �����������-���� 
;;;    ��������� ������������� ����������  � ������������� ������������ 
;;;    ������������ �� �� ����������  
;;;
;;;                                       CLIPS Version 6.3 Example
;;;
;;;********************************************************************
;;;     ��������������� �������
;;; ********************************************************************
;;; ������� ask-question ������ ������������ ������, ����������
;;;  � ���������� ?question, � �������� �� ������������ �����,
;;;  ������������� ������ ���������� �������, ��������� � $?allowed-values 

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

;;;  ������� yes-or-no-p ������ ������������ ������, ����������
;;;  � ���������� ?question, � �������� �� ������������ ����� yes(�)���
;;;  no(n). � ������ �������������� ������ ������� ���������� �������� TRUE,
;;;  ����� - FALSE

(deffunction yes-or-no-p (?question)
   (bind ?response (ask-question ?question yes no y n))
   (if (or (eq ?response yes) (eq ?response y))
       then TRUE 
       else FALSE))


;;;******************************************************************
;;;     ��������������� �������
;;;*****************************************************************






(defrule determine-what-with-bus ""
   (not (what-with-bus ?))
   (not (why-bus-off ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "������� ���� �����������(opt1), ������� ���������� ���������� ������ ������ (opt2), ���� ����������� (opt3) ? (opt1/opt2/opt3)? " 
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
	(ask-question "���� ����������� ��������� ��������������� ������ ��� ��� ���� (def/UROV)? "
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
   	(if (yes-or-no-p "��������� �� ����� ������������������� ����������� (yes/no)? ") 
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
   	(if (yes-or-no-p "��������� �� ����� ������������������� ����������� (yes/no)? ") 
       	then 
       	(assert (bus-breaker-status broken))
	   
       	else 
      	(assert (bus-breaker-status normal))))


(defrule determine-def-repair ""
   (def-or-UROV def)
   (bus-breaker-status normal)
   (not (repair ?))
   =>
   (if (yes-or-no-p "������� ���������, ��� � ����������������� ��������� �������� ����������. �������� �����������? (yes/no)? ") 
       then 
       (if (yes-or-no-p "�������� �������� � ���������� �����.�������� �� ������ ���������� �� ���� ��������� ��������� ��� ��� ��� ���?  (yes/no)? ")
           then (assert (repair "������� ���������� �� ���� ��������� ��������� ��� ��� ��� ���."))
           else (assert (repair "������� ���������� �� ���� �������, �� ����� ���������� ����� ��� �� ������ ������������� �������� ����������.")))
       else 
       (if (yes-or-no-p "�������� �� ������ ���������� �� ���� ��������� ��������� ��� ��� ��� ���?  (yes/no)? ")
           then (assert (repair "������� ���������� �� ���� ��������� ��������� ��� ��� ��� ���."))
           else (assert (repair "������� ���������� �� ���� �������, �� ����� ���������� ����� ��� �� ������ ������������� �������� ����������.")))))



(defrule determine-manually-automatics ""
   (def-or-UROV UROV)
   (bus-breaker-status normal)
   (not (repair ?))
   =>
   (if (yes-or-no-p "������� ���������, ��� � ����������������� ��������� �������� ����������. �������� �����������? (yes/no)? ") 
       then 
       (if (yes-or-no-p "�������� �������� � ���������� �����.�������� �� ������ ���������� �� ���� ��������� ��������� ��� ��� ��� ���?  (yes/no)? ")
           then (assert (repair "������� ���������� �� ���� ��������� ��������� ��� ��� ��� ���."))
           else (assert (repair "������� ���������� �� ���� �������, �� ����� ���������� ����� ��� �� ������ ������������� �������� ����������.")))
       else 
       (if (yes-or-no-p "�������� �� ������ ���������� �� ���� ��������� ��������� ��� ��� ��� ���?  (yes/no)? ")
           then (assert (repair "������� ���������� �� ���� ��������� ��������� ��� ��� ��� ���."))
           else (assert (repair "������� ���������� �� ���� �������, �� ����� ���������� ����� ��� �� ������ ������������� �������� ����������.")))))










(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "������������ �� ���������� ��������� ��������:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
