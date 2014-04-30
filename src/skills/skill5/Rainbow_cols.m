function [ Colour ] = Rainbow_cols( Code )

% ---------------------------------------------------------------------------
%
%     Generates a single colour that is taken from the LaTeX Rainbow
% dictionary.
%
%     The significance of the parameters:
%
%     Input:
%
%     - Code ( integer )
%        the Code in the Rainbow dictionary; the value must be positive and
%        smaller than the highest defined code (see the test printout); bad
%        Code-s are converted to 0; by default, 0;
%
%     Output:
%
%     - Colour ( real(3) )
%        the generated colour.
%
% ���������������������������������������������������������������������������
%
%  v 1.0 05.08.2005
%  v 1.2 27.09.2010
%  v 1.3 24.05.2011
%
% ---------------------------------------------------------------------------
%
%  � 2011 Delft University of Technology
%
% ---------------------------------------------------------------------------

   % Check the input parameters
   %
   if ( nargin < 1 )
      Code = 0;
   end

   Colours = [ [ 0.000 0.000 0.627 ] ; ... % 0
               [ 0.023 0.690 0.510 ] ; ... % 1
               [ 0.969 0.035 0.243 ] ; ... % 2
               [ 1.000 1.000 0.935 ] ; ... % 3
               [ 0.209 0.499 0.585 ] ; ... % 4
               [ 0.286 0.647 0.737 ] ; ... % 5
               [ 0.780 0.953 0.980 ] ; ... % 6
               [ 0.250 0.350 0.350 ] ; ... % 7
               [ 0.650 0.800 0.800 ] ; ... % 8
               [ 0.970 0.930 0.680 ] ; ... % 9
               [ 0.008 0.313 0.490 ] ; ... % 10
               [ 0.008 0.219 0.349 ] ; ... % 11
               [ 0.004 0.415 0.611 ] ; ... % 12
               [ 0.125 0.345 0.490 ] ; ... % 13
               [ 0.500 0.370 0.390 ] ; ... % 14
               [ 0.760 0.530 0.540 ] ; ... % 15
               [ 0.250 0.200 0.240 ] ; ... % 16
               [ 0.050 0.160 0.190 ] ; ... % 17
               [ 0.190 0.470 0.520 ] ; ... % 18
               [ 1.000 0.500 0.500 ] ; ... % 19
               [ 1.000 0.600 0.600 ] ; ... % 20
               [ 1.000 0.700 0.700 ] ; ... % 21
               [ 0.630 0.450 0.465 ] ; ... % 22
               [ 0.965 0.996 0.925 ] ; ... % 23
               [ 1.000 0.900 0.000 ] ; ... % 24
               [ 1.000 0.633 0.000 ] ; ... % 25
               [ 1.000 0.366 0.000 ] ; ... % 26
               [ 1.000 0.100 0.000 ] ; ... % 27
               [ 0.000 0.650 1.000 ] ; ... % 28
               [ 0.000 0.169 0.376 ] ; ... % 29  TU Delft / new "huisstijl"
               [ 0.678 0.776 0.063 ] ; ... % 30
               [ 0.631 0.000 0.345 ] ; ... % 31
               [ 0.063 0.545 0.851 ] ; ... % 32
               [ 0.400 0.737 0.667 ] ; ... % 33
               [ 0.000 0.576 0.671 ] ; ... % 34
               [ 0.482 0.627 0.788 ] ; ... % 35
               [ 0.467 0.753 0.843 ] ; ... % 36
               [ 0.000 0.251 0.251 ] ];    % 37

   Code = round( Code );
   if ( Code > size( Colours , 1 )-1 )
      Code = 0;
   end

   Colour = Colours( Code+1 , : );
return