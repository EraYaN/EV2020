%%Task3 van Module 5
clc
clear all
close all
%Defining the channel parameters
L = 0.5E-6;     %H
C = 2E-12;      %F
Zload = 100;    %Ohm
l = 1.2;        %m
Z_0 = sqrt (L/C);       %500 Ohm
t_w =0.5E-9;    %s

%Gamma is the reflection coefficient at the load
%gamma is propagatie coefficient
Gamma = (Zload - Z_0) /(Zload + Z_0) ;
gamma = sqrt(C*L);

%Defining matrices  
t = 0:0.05E-9:5E-9;
x = 0:0.012:1.2;
x_length = length(x) ;
t_length = length(t) ;

V = zeros(length(x),length(t)) ;
I = zeros(length(x),length(t)) ;
Power = zeros(length(x),length(t)) ;

for i = 1: x_length 
    for j = 1: t_length 
        V(i,j) = U(t(j)-gamma * x(i)) + Gamma*U(t(j)+ gamma*(x(i)-2*l)) - Gamma*U(t(j) - gamma* (x(i)+2*l)) - (Gamma^2) * U(t(j) + gamma * (x(i)-4*l));
        I(i,j) = (1/Z_0) * U(t(j)-gamma * x(i))- (Gamma/Z_0) * U(t(j) + gamma * (-2*l+x(i))) - (Gamma/Z_0) * U(t(j) + gamma * (-2*l-x(i))) + ((Gamma^2)/Z_0) + U(t(j) + gamma * (-4*l+x(i)));
    end;
end;

Bscan_plot(t, x, V, 't (s)', 'x (m)', 'V(V)', [20,45])



