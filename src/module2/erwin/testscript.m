%fclose(instrfindall);
%delete(instrfindall);
%clear;
com = Communication('COM5');
com.status()
com.drive(150,135)
com.drive()
com.delete()