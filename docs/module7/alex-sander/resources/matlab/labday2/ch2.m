function [z_2,control] = ch2(x, y, L)


while length(y) > L+length(x)-1
    % truncate
    y = y(1:length(y)-1);
end

while length(y) < L+length(x)-1
    % zero padding
    y = [y;0];
end



Ny = length(y);
Nx = length(x);
%L = Ny- Nx + 1; %wordt boven gefixt

xr = flipud(x);             %reverse the sequence x ( assuming a col vector)
z_2 = filter(xr, 1, [y]);       %matched filtering
z_2 = z_2(Nx +1 : end);         %skip the first Nx sampled, so length(h) = L
alpha = x' * x;             %compute scale
z_2 = z_2/alpha;                %scale down

xtoep = toep(x, length(y));
control = (xtoep' * y) / alpha;

end 
