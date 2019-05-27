;;;********************************************************************
;;;  
;;;    
;;;     
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

(defrule determine-breaker-status ""
   (not (breaker-status ?))
   (not (repair ?))
   =>
   (if (yes-or-no-p "�������������� ����� ������ ������������� ����������� ��� ���������� ����������� ����� (yes/no)? ") 
       	then 
       	(if (yes-or-no-p "���������� ��������� �����������. ����������� ����������? (yes/no)? ")
           then (assert (breaker-status normal))
           else (assert (breaker-status broken)))
	   
       	else 
      	(assert (breaker-status normal))))



(defrule if-the-breaker-is-broken ""
   (breaker-status broken)
   (not (local-control-button ?))
   (not (repair ?))   
   =>
   (if (yes-or-no-p "������� ���������� ���� �� ��������� ����������� � ��������� ����������� ������� �������� ����������. ����������� ����������(yes/no)?  ")
       then	
	(assert (local-control-button turned-off))            	
	             
       
       else 
	(assert (repair "��������� ����� � ����������� ������������ �� ������������ ����������� �������� ������� ���, ����� �������� �������� ����������� � � ���������� ���������� ��������� �������� � ������ ������������� ������������� �����������.")))) 
              

(defrule determine-deadend-or-transit ""
   (breaker-status normal)
   (not (deadend-or-transit ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "������������� ����� ������ �� ��������� ��� ���������� ����� (deadend/transit)? " 
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
   (if (yes-or-no-p "��� ��������� ����� � ������������� (yes/no)? ") 
       	then 	
	(assert (deadend-with-branches with-branches)) 

	else
	(assert (repair "������� ��������� �������� �������������� ���� �����, ����� ��������� ������������ ����"))))

(defrule determine-transit-with-branches ""
   (deadend-or-transit transit)
   (not (transit-with-branches ?))	
   (not (repair ?))
   =>
   (if (yes-or-no-p "��� ���������� ����� � ������������� (yes/no)? ") 
       	then 	
	(assert (transit-with-branches with-branches)) 

	else
	(assert (repair "������� ��������� � ��������� �����, ����� ������� �� � ������"))))




(defrule do-on-deadend-with-branches ""
   (deadend-with-branches with-branches)
   (not (repair ?))
   =>
 (bind ?response	
   (ask-question "�������  ������� �� ����� ������������ ��������� ������ ��� ����� �������� ��������� ����������. �������� ���������� �� ����������(option1). 
			������� �������� ���������������, ����� ������ � ��������� ���������� ������ ������� 
			��������������� (option2).����� ��������� �������� �� � �����? (option1/option2)? " 
			option1 option2))
       	
       	(if (eq ?response option1)
		then
		  (assert (repair "������� ���������� ����������"))

	else (if (eq ?response option2)
                then
		 (assert (repair "������� ������� �������� ��������������� ����� ��������� ���������� ��������� ������ ���������������")))))



(defrule do-on-transit-with-branches ""
   (transit-with-branches with-branches)
   (not (repair ?))
   =>
 (bind ?response	
   (ask-question "������� ��������� ��� ����� � ��������� ����� � ����� ������� �� ����� ������������ ��������� ������ ��� ����� �������� ��������� ����������. �������� ���������� �� ����������(option1). ������� �������� ���������������, �����
		    ������ � ��������� ���������� ������ ������� ��������������� (option2).����� ��������� �������� �� � �����? (option1/option2)? " 
			option1 option2))
       	
       	(if (eq ?response option1)
		then
		  (assert (repair "������� ���������� ����������"))

	else (if (eq ?response option2)
                then
		 (assert (repair "������� ������� �������� ���������������, ����� ��������� ���������� ��������� ������ ���������������")))))










(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "������������ �� ���������� ��������� ��������:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
