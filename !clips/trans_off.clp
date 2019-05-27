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


(defrule determine-have-damage-inf ""
   (not (line-damaged-inf ?))
   (not (repair ?))
   =>
   (if (yes-or-no-p "���� ���������� � ����� ����������� ������������  (yes/no)? ") 
       then 
       (assert (repair "������� ������� ����� � ������."))
       else 
       (assert (line-damaged-inf havent))))





(defrule determine-deadend-or-transit ""
   (line-damaged-inf havent)
   (not (deadend-or-transit ?))
   (not (repair ?))
   =>
   (bind ?response	
   (ask-question "��������� ��������� ���������� ��������� ��� ���������� ����� (deadend/transit)? " 
			deadend transit))
       	
       	(if (eq ?response deadend)
		then
		  (if (yes-or-no-p "��������� ����� �����������.����� �������� ��������� (yes/no)? ") 
       		then 
       			(assert (line-damaged havent))
       		else 
       			(assert (repair "������� ������� ����� � ������.")))

		(assert (deadend-or-transit deadend))


	else (if (eq ?response transit)
                then
		 (if (yes-or-no-p "��������� ����� �����������.����� �������� ��������� (yes/no)? ") 
       		then 
       			(assert (line-damaged havent))
       		else 
       			(assert (repair "������� ������� ����� � ������.")))

		(assert (deadend-or-transit transit)))))













(defrule print-repair ""
  (declare (salience 10))
  (repair ?item)
  =>
  (printout t crlf crlf)
  (printout t "������������ �� ���������� ��������� ��������:")
  (printout t crlf crlf)
  (format t " %s%n%n%n" ?item))
